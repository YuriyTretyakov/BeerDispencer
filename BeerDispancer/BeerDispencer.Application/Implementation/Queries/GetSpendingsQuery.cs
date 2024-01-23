using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Queries
{
    public class GetAllSpendingsQuery: IRequest<UsageResponse>
	{
		public Guid DispencerId { get; set; }
	}
}

