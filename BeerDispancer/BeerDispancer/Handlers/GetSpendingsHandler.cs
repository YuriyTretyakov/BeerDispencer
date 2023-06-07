using System;
using Beerdispancer.Domain.Abstractions;
using Beerdispancer.Domain.Entities;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.WebApi.Extensions;
using BeerDispencer.WebApi.Queries;
using BeerDispencer.WebApi.Responses;
using MediatR;

namespace BeerDispencer.WebApi.Handlers
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

        public Task<UsageResponse> Handle(GetSpendingsQuery request, CancellationToken cancellationToken)
        {
            var usagesFound = _dispencerUof.UsageRepo.GetUsagesByDispencerId(request.DispencerId);
            double total = 0;

            var spendings = usagesFound.Select(x =>
            {
                var entry = new UsageEntry
                {
                    OpenedAt = x.OpenAt,
                    ClosedAt = x.ClosedAt,
                };

                entry.FlowVolume = x.FlowVolume ?? _calculator.GetFlowVolume(DateTime.UtcNow, x.OpenAt, _beerFlowSettings.LitersPerSecond);
                entry.TotalSpent = x.TotalSpent ?? _calculator.GetTotalSpent(entry.FlowVolume, _beerFlowSettings.PricePerLiter);
                total += entry.TotalSpent ?? 0;
                return entry;
            }).ToArray();

            return Task.FromResult(new UsageResponse { Amount = total, Usages = spendings });
        }
    }
}

