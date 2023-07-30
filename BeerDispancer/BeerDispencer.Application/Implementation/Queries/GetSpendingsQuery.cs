using System;
using BeerDispancer.Application.Implementation.Response;
using MediatR;

namespace BeerDispancer.Application.Implementation.Queries
{
	public class GetAllSpendingsQuery: IRequest<UsageResponse>
	{
		public Guid DispencerId { get; set; }
	}
}

