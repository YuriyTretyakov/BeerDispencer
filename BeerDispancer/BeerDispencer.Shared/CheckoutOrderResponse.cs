using System;
namespace BeerDispencer.Shared
{
    public class CheckoutOrderResponse
    {
        public string? SessionId { get; set; }

        public string? PubKey { get; set; }
    }
}

