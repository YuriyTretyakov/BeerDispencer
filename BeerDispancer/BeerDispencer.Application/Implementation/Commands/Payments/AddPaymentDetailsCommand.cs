using System;
using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands.Payments
{
    public class AddPaymentDetailsCommand : IRequest<bool>
    {
        public StripeTokenResponse StripeData { get; set; }
        public Guid UserId { get; set; }
    }
}

