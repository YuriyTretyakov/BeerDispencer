using System;
using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Commands;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using MediatR;
using Stripe;

namespace BeerDispenser.Application.Implementation.Handlers
{
	public class ClientOperationsHandler : IRequestHandler<ClientOperationsCommand>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowSettings _beerFlowSettings;

        public ClientOperationsHandler(IDispencerUof dispencerUof, IBeerFlowSettings beerFlowSettings)
		{
            _dispencerUof = dispencerUof;
            _beerFlowSettings = beerFlowSettings;
        }

        public async Task Handle(ClientOperationsCommand request, CancellationToken cancellationToken)
        {
           
            if (request.Status == Shared.DispenserStatus.Closed)
            {
                var dispencerDto = await _dispencerUof
                   .DispencerRepo
                   .GetByIdAsync(request.Id);

                var usagesDto = await _dispencerUof.UsageRepo.GetByDispencerIdAsync(request.Id);

                var usages = usagesDto.ToDomain(_beerFlowSettings);

                var dispencer = Dispenser.CreateDispenser(
                    dispencerDto.Id,
                    dispencerDto.Volume.Value,
                    dispencerDto.Status.Value,
                    dispencerDto.IsActive.Value,
                    usages.ToList(),
                    _beerFlowSettings);

                var spendings = dispencer.GetSpendings();

                var recentUsage = spendings
                    .Usages
                    .FirstOrDefault(x => x.ClosedAt.Equals(spendings.Usages.Max(x => x.ClosedAt)));

                var amountToCharge = recentUsage.TotalSpent;
                var defaultCard = await _dispencerUof.PaymentCardRepository.GetDefaultCard(request.UserId);

                var options = new ChargeCreateOptions
                {
                    Customer = defaultCard.CustomerId,
                    Amount = (long)amountToCharge *100,
                    Currency = "usd",
                    Source = defaultCard.CardId,
                    Description = $"Payment for usage Dispenser {dispencerDto.Id} Amount: {amountToCharge} Volume: {recentUsage.FlowVolume}",
                };
                var service = new ChargeService();
               var charge =  service.Create(options);

            }
        }
    }
}

