using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using MediatR;

namespace BeerDispencer.Application.Implementation.Handlers
{
    public class GetAllDispencersHandler : IRequestHandler<GetAllDispencersQuery, DispencerDto[]>
    {
        private readonly IDispencerUOW _dispencerUof;

        public GetAllDispencersHandler(IDispencerUOW dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

         async Task<DispencerDto[]> IRequestHandler<GetAllDispencersQuery, DispencerDto[]>.Handle(GetAllDispencersQuery request, CancellationToken cancellationToken)
        {
            var allDispencers = await _dispencerUof.DispencerRepo.GetAllAsync();
            return allDispencers.ToArray();
        }
    }
}

