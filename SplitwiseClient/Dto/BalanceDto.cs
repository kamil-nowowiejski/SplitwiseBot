using Newtonsoft.Json;

namespace SplitwiseBot.SplitwiseClient.Dto
{
	public class BalanceDto
	{
		[JsonProperty("currency_code")]
		public string CurrencyCode { get; set; }

		public double Amount { get; set; }
	}
}