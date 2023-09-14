using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using MediatR;

namespace BeerDispencer.Application.Implementation.Handlers
{
    public class GetAllDispensersHandler : IRequestHandler<GetAllDispensersQuery, DispenserDto[]>
    {
        private readonly IDispencerUof _dispencerUof;

        public GetAllDispensersHandler(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

         async Task<DispenserDto[]> IRequestHandler<GetAllDispensersQuery, DispenserDto[]>.Handle(GetAllDispensersQuery request, CancellationToken cancellationToken)
        {
            var allDispencers = await _dispencerUof.DispencerRepo.GetAllAsync();
            return allDispencers.ToArray();
        }
    }
}

