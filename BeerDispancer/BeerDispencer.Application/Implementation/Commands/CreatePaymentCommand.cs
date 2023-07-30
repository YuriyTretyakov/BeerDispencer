using System;
using BeerDispencer.Application.Implementation.Response;
using MediatR;

namespace BeerDispencer.Application.Implementation.Commands
{
	public class CreatePaymentCommand:IRequest<CreatePaymentResponse>
    {
		public Guid DispencerId { get; set; }
		public double Amount { get; set; }
	}
}

