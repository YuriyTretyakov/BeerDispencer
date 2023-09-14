using System;
using BeerDispencer.Shared;
using MediatR;

namespace BeerDispencer.Application.Implementation.Queries
{
	public class GetOrderDetailsQuery:IRequest<OrderResponseDetails>
	{
		public string SessionId { get; set; }
	}
}

