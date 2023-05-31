using System;
using Newtonsoft.Json;

namespace BeerDispencer.WebApi.Commands
{
    public class UsageResponse
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("usages")]
        public UsageEntry[] Usages { get; set; }
    }

	public class UsageEntry
    {
		
        [JsonProperty("opened_at")]
        public DateTime? OpenedAt { get; set; }

        [JsonProperty("closed_at")]
        public DateTime? ClosedAt { get; set; }

        [JsonProperty("flow_volume")]
        public double? FlowVolume { get; set; }

        [JsonProperty("total_spent")]
        public double? TotalSpent { get; set; }

    }
}


