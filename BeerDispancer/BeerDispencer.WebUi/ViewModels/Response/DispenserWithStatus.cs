
using BeerDispenser.Shared;
using Newtonsoft.Json;

namespace BeerDispenser.WebUi.ViewModels.Response
{
    public class DispenserWithStatus
	{
        public Guid Id { get; set; }
        public decimal Volume { get; set; }
        [JsonProperty("status")]
        public DispenserStatusDto Status { get; set; }
        public string ReservedFor { get; set; }

        public bool IsActive { get; set; }
        public bool AllowReservation =>
            (string.IsNullOrEmpty(ReservedFor) && Status == DispenserStatusDto.Closed);
    }
}

