using System;
using BeerDispencer.WebApi.Responses;
using MediatR;

namespace BeerDispencer.WebApi.Queries
{
	public class GetSpendingsQuery: IRequest<UsageResponse>
	{
		public Guid DispencerId { get; set; }
	}
}

