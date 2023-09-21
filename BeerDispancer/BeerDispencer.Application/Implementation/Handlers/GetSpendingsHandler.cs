using BeerDispencer.Domain.Abstractions;
using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.Implementation.Queries;
using MediatR;
using BeerDispencer.Application;
using BeerDispencer.Shared;
using BeerDispenser.Domain.Entity;

namespace BeerDispancer.Application.Implementation.Handlers
{
    public class GetSpendingsHandler : IRequestHandler<GetAllSpendingsQuery, UsageResponse>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowSettings _beerFlowSettings;

        public GetSpendingsHandler(IDispencerUof dispencerUof, IBeerFlowSettings beerFlowSettings)
		{
            _dispencerUof = dispencerUof;
            _beerFlowSettings = beerFlowSettings;
        }

        public async Task<UsageResponse> Handle(GetAllSpendingsQuery request, CancellationToken cancellationToken)
        {
            var dispencerDto = await _dispencerUof
                    .DispencerRepo
                    .GetByIdAsync(request.DispencerId);

            if (dispencerDto is null)
            {
                return null;
            }

            var usagesDto = await _dispencerUof.UsageRepo.GetByDispencerIdAsync(request.DispencerId);
            

            var usages = usagesDto.ToDomain(_beerFlowSettings);

            var dispencer = Dispenser.CreateDispenser(
            dispencerDto.Id,
                dispencerDto.Volume.Value,
                dispencerDto.Status.Value,
                usages.ToList(),
                _beerFlowSettings);


            return dispencer.GetSpendings();
        }
    }
}

