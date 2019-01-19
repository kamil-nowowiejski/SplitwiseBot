using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using SplitwiseBot.Constants;
using SplitwiseBot.Infrastructure;
using SplitwiseBot.SplitwiseClient;
using SplitwiseBot.SplitwiseClient.Dto;

namespace SplitwiseBot.BotActions
{
	public class GetMembersListAction : BotAction
	{
		private readonly IConfiguration _configuration;

		public GetMembersListAction(UsersRegistry usersRegistry, SplitwiseClientBuilder splitwiseClientBuilder,
			IConfiguration configuration) 
			: base(usersRegistry, splitwiseClientBuilder)
		{
			_configuration = configuration;
		}

		protected override bool RequiresAuthenticatedUser => true;

		protected override void PerformActionInternal(string userId, Activity reply, string message)
		{
			string groupName = _configuration[ConfigurationKeys.GroupName];
			PrepareReplyForMostIndebtedGroupMembers(userId, reply, groupName);
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
	}
}