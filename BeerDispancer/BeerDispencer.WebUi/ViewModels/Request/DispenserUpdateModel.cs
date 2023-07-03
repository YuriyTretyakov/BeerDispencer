using System;
using System.Net;
using Newtonsoft.Json;
namespace BeerDispencer.WebUi.ViewModels.Request

{
    public class DispenserUpdateModel
    {
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("status")]
        public DispencerStatusDto Status { get; set; }
    }
}

