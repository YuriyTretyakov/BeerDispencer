using BeerDispencer.Shared;
using MediatR;

namespace BeerDispencer.Application.Implementation.Commands
{
    public class DispencerPrePayCommand: NewOrderDetails,IRequest<CheckoutOrderResponse>
    {
        public string WebApiBaseUrl { get; set; }

        public string WebUiBaseUrl { get; set; }
    }
}

