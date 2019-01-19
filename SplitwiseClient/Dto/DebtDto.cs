using Newtonsoft.Json;

namespace SplitwiseBot.SplitwiseClient.Dto
{
	public class DebtDto
	{
		public long From { get; set; }

		public long To { get; set; }

		public double Amount { get; set; }

		[JsonProperty("currency_code")]
		public string CurrencyCode { get; set; }
	}
}