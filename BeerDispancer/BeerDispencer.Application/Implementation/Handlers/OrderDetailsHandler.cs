using System;
using BeerDispencer.Application.Implementation.Commands;
using BeerDispencer.Application.Implementation.Queries;
using BeerDispencer.Shared;
using MediatR;
using Stripe.Checkout;

namespace BeerDispencer.Application.Implementation.Handlers
{
	public class OrderDetailsHandler: IRequestHandler<GetOrderDetailsQuery, OrderResponseDetails>
    {
		public OrderDetailsHandler()
		{
		}

        public Task<OrderResponseDetails> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            var sessionService = new SessionService();
            var session = sessionService.Get(request.SessionId);

            return Task.FromResult(new OrderResponseDetails
            {
                AmountTotal = session.AmountTotal.Value,
                Email = session.CustomerDetails.Email,
                IsSuccess = true
            }) ;

        }
    }
}

