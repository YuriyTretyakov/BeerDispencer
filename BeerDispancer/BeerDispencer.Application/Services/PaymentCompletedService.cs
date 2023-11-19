﻿using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Messaging.Consumers;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using BeerDispenser.Kafka.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeerDispenser.Application.Services
{
    public class PaymentCompletedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PaymentCompletedConsumer _completedEventConsumer;
        private readonly IBeerFlowSettings _beerFlowSettings;
        CancellationToken _cancellationToken;

        public PaymentCompletedService(
            IServiceScopeFactory serviceScopeFactory,
            PaymentCompletedConsumer completedEventConsumer,
            IBeerFlowSettings beerFlowSettings)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _completedEventConsumer = completedEventConsumer;
            _beerFlowSettings = beerFlowSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationToken = stoppingToken;
            _completedEventConsumer.StartConsuming(stoppingToken);
            ProcessConsumingAsync();
        }

        private async Task ProcessConsumingAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var message =  await _completedEventConsumer.ConsumeAsync(_cancellationToken);

                if (message is not null)
                {
                    await ProcessPaymentAsync(message);
                }
            }
            _completedEventConsumer.Stop(_cancellationToken);
            _completedEventConsumer.Dispose();
        }

        private async Task ProcessPaymentAsync(
            IReadonlyEventHolder<PaymentCompletedEvent> message)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IDispencerUof>();

            using (var transaction = uow.StartTransaction())
            {
                var dispencerDto = await uow
                  .DispencerRepo
                  .GetByIdAsync(message.Event.OriginalEvent.DIspenserId);

                var usagesDto = await uow.UsageRepo.GetByDispencerIdAsync(dispencerDto.Id);

                var usages = usagesDto.ToDomain(_beerFlowSettings);

                var dispenser = Dispenser.CreateDispenser(
                    dispencerDto.Id,
                    dispencerDto.Volume.Value,
                    dispencerDto.Status.Value,
                    dispencerDto.IsActive.Value,
                    usages.ToList(),
                    _beerFlowSettings);

                var usageDto = dispenser.Close().ToDto();

                usageDto.PaidBy = message.Event.OriginalEvent.PaymentInitiatedBy;
                usageDto.PaymentStatus = message.Event.Status;
                usageDto.Reason = message.Event.Reason;

                await uow.UsageRepo.UpdateAsync(usageDto);

                await uow.DispencerRepo.UpdateAsync(dispenser.ToDto());
                await uow.Complete();
                uow.CommitTransaction();
            }

        }
    }
}

