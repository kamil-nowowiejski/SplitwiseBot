using Microsoft.Bot.Schema;
using SplitwiseBot.Exceptions;
using SplitwiseBot.Infrastructure;
using SplitwiseBot.SplitwiseClient;

namespace SplitwiseBot.BotActions
{
	public abstract class BotAction
	{
		protected UsersRegistry UsersRegistry { get; }
		protected SplitwiseClientBuilder SplitwiseClientBuilder { get; }

		protected abstract bool RequiresAuthenticatedUser { get; }

		protected BotAction(UsersRegistry usersRegistry, SplitwiseClientBuilder splitwiseClientBuilder)
		{
			UsersRegistry = usersRegistry;
			SplitwiseClientBuilder = splitwiseClientBuilder;
		}

		public void PerformAction(string userId, Activity reply, string message)
		{
			if (RequiresAuthenticatedUser && !UsersRegistry.IsUserRegistered(userId))
			{
				throw new UserNotAuthenticatedException();
			}

			PerformActionInternal(userId, reply, message);
		}

		protected abstract void PerformActionInternal(string userId, Activity reply, string message);

		protected SplitwiseClient.SplitwiseClient CreateSplitwiseClient(string userId)
		{
			(string accessToken, string accessTokenSecret) = UsersRegistry.GetAccessTokenWithSecret(userId);
			return SplitwiseClientBuilder.Build(accessToken, accessTokenSecret);
		}
	}
}