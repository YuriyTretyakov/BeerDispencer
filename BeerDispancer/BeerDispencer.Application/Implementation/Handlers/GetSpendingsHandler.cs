using BeerDispencer.Domain.Abstractions;
using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.Implementation.Queries;
using MediatR;
using BeerDispencer.Application;
using BeerDispencer.Domain.Entity;
using BeerDispencer.Shared;

namespace BeerDispancer.Application.Implementation.Handlers
{
    public class GetSpendingsHandler : IRequestHandler<GetAllSpendingsQuery, UsageResponse>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowCalculator _calculator;

        public GetSpendingsHandler(IDispencerUof dispencerUof,IBeerFlowCalculator calculator)
		{
            _dispencerUof = dispencerUof;
            _calculator = calculator;
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
            

            var usages = usagesDto.ToDomain();

            var dispencer = Dispencer.Create(
            dispencerDto.Id.Value,
                dispencerDto.Volume.Value,
                dispencerDto.Status.Value,
                usages.ToList());


            return dispencer.GetSpendings(_calculator);
        }
    }
}

