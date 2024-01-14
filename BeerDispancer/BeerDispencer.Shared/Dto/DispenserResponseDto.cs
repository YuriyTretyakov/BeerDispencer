using Newtonsoft.Json;

namespace BeerDispenser.Shared.Dto
{
    public class DispenserResponseDto
    {
        public Guid Id { get; set; }
        public decimal Volume { get; set; }
        [JsonProperty("status")]
        public DispenserStatusDto Status { get; set; }
        public string ReservedFor { get; set; }

        public bool IsActive { get; set; }
        public bool AllowReservation =>
            string.IsNullOrEmpty(ReservedFor) && Status == DispenserStatusDto.Closed;
    }
}

