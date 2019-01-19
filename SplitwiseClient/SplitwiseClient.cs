using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using SplitwiseBot.SplitwiseClient.Dto;

namespace SplitwiseBot.SplitwiseClient
{
	public class SplitwiseClient
	{
		private readonly RestClient _restClient;

		internal SplitwiseClient(
			string baseUri, 
			string consumerKey, 
			string consumerSecret, 
			string accessToken, 
			string accessTokenSecret) 
		{
			_restClient = new RestClient(baseUri);
			_restClient.Authenticator =
				OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, accessToken, accessTokenSecret);
		}

		public List<GroupDto> GetGroups()
		{
			RestRequest request = new RestRequest("get_groups", Method.GET);
			var response = ExecuteRequest(request);

			dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Content);
			return JsonConvert.DeserializeObject<List<GroupDto>>(data.groups.ToString());
		}

		private IRestResponse ExecuteRequest(RestRequest request)
		{
			var response = _restClient.Execute(request);
			if (!response.IsSuccessful && response.StatusCode == HttpStatusCode.Unauthorized)
			{
				throw new AuthenticationException();
			}

			return response;
		}
	}
}