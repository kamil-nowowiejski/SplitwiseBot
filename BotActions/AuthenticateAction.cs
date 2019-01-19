using System.Collections.Generic;
using Microsoft.Bot.Schema;
using SplitwiseBot.Infrastructure;
using SplitwiseBot.SplitwiseClient;

namespace SplitwiseBot.BotActions
{
	public class AuthenticateAction : BotAction
	{
		public AuthenticateAction(UsersRegistry usersRegistry, SplitwiseClientBuilder splitwiseClientBuilder) 
			: base(usersRegistry, splitwiseClientBuilder)
		{}

		protected override bool RequiresAuthenticatedUser => false;

		protected override void PerformActionInternal(string userId, Activity reply, string message)
		{
			var oAuthClient = SplitwiseClientBuilder.BuildOAuthClient();
			(string requestToken, string requestTokenSecret) = oAuthClient.GetRequestToken();

			UsersRegistry.SaveRequestToken(userId, requestToken, requestTokenSecret);

			string authorizationUrl = oAuthClient.GetAuthorizationUrl(requestToken);

			var singinCard = CreateSinginCard(authorizationUrl);
			reply.Attachments.Add(singinCard.ToAttachment());
		}

		private SigninCard CreateSinginCard(string authorizationUrl)
		{
			var cardAction = new CardAction()
			{
				Type = "signin",
				Value = authorizationUrl,
				Title = "Sign in to Splitwise"
			};

			return new SigninCard(string.Empty, new List<CardAction>() { cardAction });
		}
	}
}