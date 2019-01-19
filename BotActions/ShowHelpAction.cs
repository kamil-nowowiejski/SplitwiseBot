using System;
using Microsoft.Bot.Schema;
using SplitwiseBot.Infrastructure;
using SplitwiseBot.SplitwiseClient;

namespace SplitwiseBot.BotActions
{
	public class ShowHelpAction : BotAction
	{
		public ShowHelpAction(UsersRegistry usersRegistry, SplitwiseClientBuilder splitwiseClientBuilder) 
			: base(usersRegistry, splitwiseClientBuilder)
		{}

		protected override bool RequiresAuthenticatedUser => false;

		protected override void PerformActionInternal(string userId, Activity reply, string message)
		{
			reply.Text = $"Command \"{message}\" is unknown.{Environment.NewLine}" +
			             $"Only supported comment right now is: \"list\"";
		}
	}
}