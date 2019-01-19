using System.Collections.Generic;
using Newtonsoft.Json;

namespace SplitwiseBot.SplitwiseClient.Dto
{
	public class MemberDto
	{
		public long Id { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		public string Email { get; set; }

		public List<BalanceDto> Balance { get; set; }
	}
}
