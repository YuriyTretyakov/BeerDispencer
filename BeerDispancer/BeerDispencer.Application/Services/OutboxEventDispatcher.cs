using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Kafka.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeerDispenser.Application.Services
{
    public class OutboxEventDispatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PaymentToProcessPublisher _eventsTrigger;
        private readonly ILogger<OutboxEventDispatcher> _logger;
        private ManualResetEvent _ready;

        public OutboxEventDispatcher(
            IServiceScopeFactory serviceScopeFactory,
            PaymentToProcessPublisher eventsTrigger,
            ILogger<OutboxEventDispatcher> logger,
            IHostApplicationLifetime lifetime)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _eventsTrigger = eventsTrigger;
            _logger = logger;
           
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            { 
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var uow = scope.ServiceProvider.GetRequiredService<IDispencerUof>();

                    using var transaction = uow.StartTransaction();
                    {
                        var eventsToProcess = await uow.OutboxRepo.GetNotProccessedEvents();

                        foreach (var @event in eventsToProcess)
                        {
                            await ProcessEventAsync(@event, uow, stoppingToken);

                        }
                        await uow.Complete();
                        uow.CommitTransaction();
                    }
                   
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
        }


        private async Task ProcessEventAsync(OutboxDto outboxEvent, IDispencerUof uow,  CancellationToken cancellationToken)
        {
            try
            {
                if (outboxEvent.EventType == typeof(EventHolder<PaymentToProcessEvent>).ToString())
                {
                    await ProcessPaymentEventAsync(outboxEvent, cancellationToken);
                }

                outboxEvent.EventState = EventStateDto.Completed;
                outboxEvent.UpdatedAt = DateTime.UtcNow;
                await uow.OutboxRepo.UpdateAsync(outboxEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while executing {method}. Exception: {@ex}", nameof(ProcessEventAsync), ex);
            }
        }

        private async Task ProcessPaymentEventAsync(OutboxDto outboxEvent, CancellationToken cancellationToken)
        {
            var paymentEvent = JsonConvert.DeserializeObject<EventHolder<PaymentToProcessEvent>>(outboxEvent.Payload);
            await _eventsTrigger.RaiseEventAsync(paymentEvent, cancellationToken);
        }
    }
}

