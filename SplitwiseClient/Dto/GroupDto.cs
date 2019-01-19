using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SplitwiseBot.SplitwiseClient.Dto
{
	public class GroupDto
	{
		public long Id { get; set; }
		public string Name { get; set; }

		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }

		public List<MemberDto> Members { get; set; }

		[JsonProperty("simplify_by_default")]
		public bool SimplifyByDefault { get; set; }

		[JsonProperty("original_debts")]
		public List<DebtDto> OriginalDebts { get; set; }

		[JsonProperty("simplified_debts")]
		public List<DebtDto> SimplifyDebts { get; set; }
	}
}