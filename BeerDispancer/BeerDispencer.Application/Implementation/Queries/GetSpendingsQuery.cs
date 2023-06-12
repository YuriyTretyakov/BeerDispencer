using System;
using BeerDispancer.Application.Implementation.Response;
using MediatR;

namespace BeerDispancer.Application.Implementation.Queries
{
	public class GetSpendingsQuery: IRequest<UsageResponse>
	{
		public Guid DispencerId { get; set; }
	}
}

