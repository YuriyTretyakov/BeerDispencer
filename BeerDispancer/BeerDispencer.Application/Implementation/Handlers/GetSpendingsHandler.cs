using System;
using Beerdispancer.Domain.Abstractions;
using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.Implementation.Queries;
using BeerDispancer.Application.Implementation.Response;
using BeerDispencer.Application.Abstractions;
using MediatR;

namespace BeerDispancer.Application.Implementation.Handlers
{
	public class GetSpendingsHandler : IRequestHandler<GetSpendingsQuery, UsageResponse>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowSettings _beerFlowSettings;
        private readonly IBeerFlowCalculator _calculator;

        public GetSpendingsHandler(IDispencerUof dispencerUof, IBeerFlowSettings beerFlowSettings,IBeerFlowCalculator calculator)
		{
            _dispencerUof = dispencerUof;
            _beerFlowSettings = beerFlowSettings;
            _calculator = calculator;
        }

        public async Task<UsageResponse> Handle(GetSpendingsQuery request, CancellationToken cancellationToken)
        {
            var usagesFound = await _dispencerUof.UsageRepo.GetByDispencerIdAsync(request.DispencerId);
            double total = 0;

            var spendings = usagesFound.Select(x =>
            {
                var entry = new UsageEntry
                {
                    OpenedAt = x.OpenAt,
                    ClosedAt = x.ClosedAt,
                };

                entry.FlowVolume = x.FlowVolume ?? _calculator.GetFlowVolume(DateTime.UtcNow, x.OpenAt);
                entry.TotalSpent = x.TotalSpent ?? _calculator.GetTotalSpent(entry.FlowVolume);
                total += entry.TotalSpent ?? 0;
                return entry;
            }).ToArray();

            return new UsageResponse { Amount = total, Usages = spendings };
        }
    }
}

