namespace SplitwiseBot.SplitwiseClient
{
	public class SplitwiseClientBuilder
	{
		private readonly string _consumerKey;
		private readonly string _consumerSecret;
		private readonly string _baseUri;
		private readonly string _callbackUrl;
		private readonly string _getRequestTokenMetod;
		private readonly string _getAccessTokenMetod;
		private readonly string _authorizationUrl;

		public SplitwiseClientBuilder(string consumerKey, string consumerSecret, 
			string baseUri, string callbackUrl, string getRequestTokenMetod, string getAccessTokenMetod,
			string authorizationUrl)
		{
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
			_baseUri = baseUri;
			_callbackUrl = callbackUrl;
			_getRequestTokenMetod = getRequestTokenMetod;
			_getAccessTokenMetod = getAccessTokenMetod;
			_authorizationUrl = authorizationUrl;
		}

		public SplitwiseClient Build(string accessToken, string accessTokenSecret)
		{
			return new SplitwiseClient(_baseUri, _consumerKey, _consumerSecret, accessToken, accessTokenSecret);
		}

		public OAuthClient BuildOAuthClient()
		{
			return new OAuthClient(_consumerKey, _consumerSecret, _baseUri, _callbackUrl, _getRequestTokenMetod, _getAccessTokenMetod, _authorizationUrl);
		}
	}
}