using BeerDispencer.Shared;
using MediatR;

namespace BeerDispancer.Application.Implementation.Queries
{
    public class GetAllSpendingsQuery: IRequest<UsageResponse>
	{
		public Guid DispencerId { get; set; }
	}
}

