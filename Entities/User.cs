namespace SplitwiseBot.Entities
{
	public class User
	{
		public string Id { get; set; }

		public string OAuthRequestToken { get; set; }
		public string OAuthRequestTokenSecret { get; set; }

		public string OAuthAccessToken { get; set; }
		public string OAuthAccessTokenSecret { get; set; }
	}
}