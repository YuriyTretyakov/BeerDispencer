using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Queries;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers
{
    public class GetActiveDispensersHandler : IRequestHandler<GetActiveDispensersQuery, DispenserDto[]>
    {
        private readonly IDispencerUof _dispencerUof;

        public GetActiveDispensersHandler(IDispencerUof dispencerUof)
        {
            _dispencerUof = dispencerUof;
        }

        async Task<DispenserDto[]> IRequestHandler<GetActiveDispensersQuery, DispenserDto[]>.Handle(GetActiveDispensersQuery request, CancellationToken cancellationToken)
        {
            var allDispencers = await _dispencerUof.DispencerRepo.GetAllAsync();
            return allDispencers.Where(x=>x.IsActive==true).ToArray();
        }
    
	}
}

