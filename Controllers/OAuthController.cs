using Microsoft.AspNetCore.Mvc;
using SplitwiseBot.Infrastructure;
using SplitwiseBot.SplitwiseClient;

namespace SplitwiseBot.Controllers
{
	[Route("[controller]")]
	public class OAuthController : Controller
	{
		private readonly SplitwiseClientBuilder _splitwiseClientBuilder;
		private readonly UsersRegistry _usersRegistry;

		public OAuthController(SplitwiseClientBuilder splitwiseClientBuilder, UsersRegistry usersRegistry)
		{
			_splitwiseClientBuilder = splitwiseClientBuilder;
			_usersRegistry = usersRegistry;
		}

		[HttpGet]
		public string SaveAccessToken(string oauth_token, string oauth_verifier)
		{
			var oAuthClient = _splitwiseClientBuilder.BuildOAuthClient();
			string requestSecret = _usersRegistry.GetRequestSecret(oauth_token);

			(string accessToken, string accessTokenSecret) = 
				oAuthClient.GetAccessToken(oauth_token, requestSecret, oauth_verifier);

			_usersRegistry.SaveAccessToken(oauth_token, accessToken, accessTokenSecret);
			return "Authorization successful!";
		}	
	}
}