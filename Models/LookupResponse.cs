using System;

namespace binlookup.Models
{
	public class LookupResponse
	{
		public string CardScheme { get; }
		public string Country { get; }
		public string Currency { get; }

		public LookupResponse(
			string cardScheme,
			string country,
			string currency)
		{
			this.CardScheme = cardScheme;
			this.Country = country;
			this.Currency = currency;
		}

		public override string ToString()
		{
			return $"LookupRequest[CardScheme: {this.CardScheme}, Country: {this.Country}, Currency:{this.Currency}]";
		}
	}
}
