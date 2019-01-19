// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SplitwiseBot.BotState;
using SplitwiseBot.Constants;
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
		private readonly SplitwiseClientBuilder _splitwiseClientBuilder;
		private readonly IConfiguration _configuration;
		private readonly ILogger _logger;
		private readonly UsersRegistry _usersRegistry;

		public SplitwiseBot( 
			ILoggerFactory loggerFactory,			 
			UsersRegistry usersRegistry, 
			SplitwiseClientBuilder splitwiseClientBuilder,
			IConfiguration configuration)
		{
			if (loggerFactory == null)
			{
				throw new System.ArgumentNullException(nameof(loggerFactory));
			}
			
			_usersRegistry = usersRegistry;
			_splitwiseClientBuilder = splitwiseClientBuilder;
			_configuration = configuration;

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

			if (_usersRegistry.IsUserRegistered(userId))
			{
				try
				{
					PrepareReplyForMostIndebtedGroupMembers(userId, reply, _configuration[ConfigurationKeys.GroupName]);
				}
				catch (AuthenticationException)
				{
					Authenticate(userId, reply);
				}
			}
			else
			{
				Authenticate(userId, reply);
			}

			


			await turnContext.SendActivityAsync(reply, cancellationToken);
		}

		private void Authenticate(string userId, Activity reply)
		{
			var oAuthClient = _splitwiseClientBuilder.BuildOAuthClient();
			(string requestToken, string requestTokenSecret) = oAuthClient.GetRequestToken();

			_usersRegistry.SaveRequestToken(userId, requestToken, requestTokenSecret);

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

		private void PrepareReplyForMostIndebtedGroupMembers(string userId, Activity reply, string splitwiseGroupName)
		{
			var mostIndeptMemebers = GetMostIndebtedGroupMembers(userId, splitwiseGroupName);
			var textSummaryCollection = mostIndeptMemebers
				.Select(m => $"{m.FirstName} {m.LastName}: {GetBalanceForLocalCurrency(m).Amount}");

			reply.Text = string.Join(Environment.NewLine, textSummaryCollection);
		}

		private IOrderedEnumerable<MemberDto> GetMostIndebtedGroupMembers(string userId, string splitwiseGroupName)
		{
			var client = CreateSplitwiseClient(userId);
			List<GroupDto> groupDtos = client.GetGroups();

			var selectedGroup = groupDtos.Single(g => g.Name == splitwiseGroupName);

			return selectedGroup.Members.OrderBy(m => GetBalanceForLocalCurrency(m).Amount);
		}

		private BalanceDto GetBalanceForLocalCurrency(MemberDto member)
		{
			return member.Balance.Single(b => b.CurrencyCode == _configuration[ConfigurationKeys.LocalCurrencyCode]);
		}

		private SplitwiseClient.SplitwiseClient CreateSplitwiseClient(string userId)
		{
			(string accessToken, string accessTokenSecret) = _usersRegistry.GetAccessTokenWithSecret(userId);
			return _splitwiseClientBuilder.Build(accessToken, accessTokenSecret);
		}
	}
}
