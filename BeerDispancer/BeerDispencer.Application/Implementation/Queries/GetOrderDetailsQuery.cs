using System;
using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Queries
{
	public class GetOrderDetailsQuery:IRequest<OrderResponseDetails>
	{
		public string SessionId { get; set; }
	}
}

