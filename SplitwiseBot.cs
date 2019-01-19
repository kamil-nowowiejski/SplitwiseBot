// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SplitwiseBot.BotActions;
using SplitwiseBot.Constants;
using SplitwiseBot.Exceptions;
using SplitwiseBot.Infrastructure;
using SplitwiseBot.SplitwiseClient;
using SplitwiseBot.SplitwiseClient.Dto;

namespace SplitwiseBot
{
	/// <summary>
	/// Represents a bot that processes incoming activities.
	/// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
	/// This is a Transient lifetime service.  Transient lifetime services are created
	/// each time they're requested. For each Activity received, a new instance of this
	/// class is created. Objects that are expensive to construct, or have a lifetime
	/// beyond the single turn, should be carefully managed.
	/// For example, the <see cref="MemoryStorage"/> object and associated
	/// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
	/// </summary>
	/// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
	internal class SplitwiseBot : IBot
	{
		private readonly ILogger _logger;
		private readonly UsersRegistry _usersRegistry;
		private readonly BotActionsRegistry _botActionsRegistry;


		public SplitwiseBot( 
			ILoggerFactory loggerFactory,			 
			UsersRegistry usersRegistry,
			BotActionsRegistry botActionsRegistry)
		{
			if (loggerFactory == null)
			{
				throw new System.ArgumentNullException(nameof(loggerFactory));
			}
			
			_usersRegistry = usersRegistry;
			_botActionsRegistry = botActionsRegistry;
			_logger = loggerFactory.CreateLogger<SplitwiseBot>();
			_logger.LogTrace("Turn start.");
		}

		/// <summary>
		/// Every conversation turn for our Echo Bot will call this method.
		/// There are no dialogs used, since it's "single turn" processing, meaning a single
		/// request and response.
		/// </summary>
		/// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
		/// for processing this conversation turn. </param>
		/// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
		/// or threads to receive notice of cancellation.</param>
		/// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
		/// <seealso cref="BotStateSet"/>
		/// <seealso cref="ConversationState"/>
		/// <seealso cref="IMiddleware"/>
		public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (turnContext.Activity.Type != ActivityTypes.Message)
			{
				return;
			}

			string userId = turnContext.Activity.From.Id;
			var reply = turnContext.Activity.CreateReply();
			var userMessage = turnContext.Activity.Text;

			BotAction action;

			try
			{
				action = _botActionsRegistry.GetAction(userMessage);
				action.PerformAction(userId, reply, userMessage);
			}
			catch (UserNotAuthenticatedException)
			{
				action = _botActionsRegistry.GetAction(BotActionCommand.Authenticate);
				action.PerformAction(userId, reply, userMessage);
			}
			catch (UnknownCommandException e)
			{
				action = _botActionsRegistry.GetAction(BotActionCommand.Help);
				action.PerformAction(userId, reply, e.Command);
			}


			await turnContext.SendActivityAsync(reply, cancellationToken);
		}
	}
}
