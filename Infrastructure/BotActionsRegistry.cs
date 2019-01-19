using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SplitwiseBot.BotActions;
using SplitwiseBot.Exceptions;

namespace SplitwiseBot.Infrastructure
{
	public class BotActionsRegistry
	{
		private readonly Dictionary<List<string>, Type> _botCommandsWithActions;
		private readonly IServiceProvider _serviceProvider;

		public BotActionsRegistry(
			Dictionary<List<string>, Type> botCommandsWithActions,
			IServiceProvider serviceProvider)
		{
			_botCommandsWithActions = botCommandsWithActions;
			_serviceProvider = serviceProvider;
		}

		public BotAction GetAction(string command)
		{
			var actionType = _botCommandsWithActions.SingleOrDefault(x => x.Key.Contains(command)).Value;
			if (actionType == null)
			{
				throw new UnknownCommandException(command);
			}

			return (BotAction) _serviceProvider.CreateScope().ServiceProvider.GetService(actionType);
		}
	}
}