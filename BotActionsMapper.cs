using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SplitwiseBot.BotActions;
using SplitwiseBot.Constants;

namespace SplitwiseBot
{
	public class BotActionsMapper
	{
		public static Dictionary<List<string>, Type> CreateMap(IServiceCollection services)
		{
			return new Dictionary<List<string>, Type>
			{
				{
					new List<string> {BotActionCommand.Authenticate},
					typeof(AuthenticateAction)
				},

				{
					new List<string> {BotActionCommand.Help},
					typeof(ShowHelpAction)
				},

				{
					new List<string> {"list"},
					typeof(GetMembersListAction)
				},
			};
		}

		public static void RegisterActions(IServiceCollection services)
		{
			services.AddSingleton<AuthenticateAction>();
			services.AddSingleton<ShowHelpAction>();
			services.AddSingleton<GetMembersListAction>();
		}
	}
}