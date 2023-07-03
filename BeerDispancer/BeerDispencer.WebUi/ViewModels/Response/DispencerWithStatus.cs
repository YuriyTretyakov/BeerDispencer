using System;
using BeerDispencer.WebUi.ViewModels.Request;
using Newtonsoft.Json;

namespace BeerDispencer.WebUi.ViewModels.Response
{
	public class DispencerWithStatus
	{
        public Guid Id { get; set; }
        public double Volume { get; set; }
        [JsonProperty("status")]
        public DispencerStatusDto Status { get; set; }
    }
}

