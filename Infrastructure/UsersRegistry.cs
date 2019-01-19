using System.Collections.Generic;
using System.Linq;
using SplitwiseBot.Entities;

namespace SplitwiseBot.Infrastructure
{
	public class UsersRegistry
	{
		private readonly List<User> _users;

		public UsersRegistry()
		{
			_users = new List<User>();
		}

		public void SaveRequestToken(string userId, string requestToken, string secret)
		{
			User user = _users.SingleOrDefault(u => u.Id == userId) ?? AddUser(userId);
			user.OAuthRequestToken = requestToken;
			user.OAuthRequestTokenSecret = secret;
		}

		public void SaveAccessToken(string requestToken, string accessToken, string secret)
		{
			User user = _users.Single(u => u.OAuthRequestToken == requestToken);
			user.OAuthAccessToken = accessToken;
			user.OAuthAccessTokenSecret = secret;
		}

		public string GetRequestSecret(string requestToken)
		{
			return _users.Single(u => u.OAuthRequestToken == requestToken).OAuthRequestTokenSecret;
		}

		public (string accessToken, string accessTokenSecret) GetAccessTokenWithSecret(string userId)
		{
			User user = _users.Single(u => u.Id == userId);
			return (user.OAuthAccessToken, user.OAuthAccessTokenSecret);
		}

		public bool IsUserRegistered(string userId)
		{
			return _users.Any(u => u.Id == userId);
		}

		private User AddUser(string userId)
		{
			var user = new User { Id = userId };
			_users.Add(user);
			return user;
		}
	}
}