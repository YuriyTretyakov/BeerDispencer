using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands
{
    public class DispenserPrePayCommand: NewOrderDetails,IRequest<CheckoutOrderResponse>
    {
        public string WebApiBaseUrl { get; set; }

        public string WebUiBaseUrl { get; set; }
    }
}

