using System;
using RestSharp;
using RestSharp.Authenticators;

namespace SplitwiseBot.SplitwiseClient
{
	public class OAuthClient
	{
		private readonly string _consumerKey;
		private readonly string _consumerSecret;
		private readonly string _baseUri;
		private readonly string _callbackUrl;
		private readonly string _getRequestToken;
		private readonly string _getAccessToken;
		private readonly string _authorizationUrl;


		internal OAuthClient(string consumerKey, string consumerSecret, string baseUri, string callbackUrl,
			string getRequestToken, string getAccessToken, string authorizationUrl)
		{
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
			_baseUri = baseUri;
			_callbackUrl = callbackUrl;
			_getRequestToken = getRequestToken;
			_getAccessToken = getAccessToken;
			_authorizationUrl = authorizationUrl;
		}

		public string GetAuthorizationUrl(string requestToken)
		{
			return $"{_authorizationUrl}?oauth_token={requestToken}";
		}

		public (string token, string tokenSecret) GetAccessToken(string requestToken, string requestTokenSecret, string verifier)
		{
			var client = new RestClient(_baseUri);
			client.Authenticator =
				OAuth1Authenticator.ForAccessToken(_consumerKey, _consumerSecret, requestToken, requestTokenSecret, verifier);
			RestRequest request = new RestRequest(_getAccessToken, Method.POST);
			var response = client.Execute(request);

			return GetTokenInfo(response);
		}

		public (string token, string tokenSecret) GetRequestToken()
		{
			var client = new RestClient(_baseUri);
			client.Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret, _callbackUrl);
			var request = new RestRequest(_getRequestToken, Method.POST);
			var response = client.Execute(request);

			return GetTokenInfo(response);
		}

		private static (string token, string tokenSecret) GetTokenInfo(IRestResponse response)
		{
			if (!response.IsSuccessful)
			{
				throw new InvalidOperationException();
			}
			return (GetQueryParameter(response.Content, "oauth_token"), GetQueryParameter(response.Content, "oauth_token_secret"));
		}

		private static string GetQueryParameter(string input, string parameterName)
		{
			foreach (string item in input.Split('&'))
			{
				var parts = item.Split('=');
				if (parts[0] == parameterName)
				{
					return parts[1];
				}
			}
			return string.Empty;
		}
	}
}