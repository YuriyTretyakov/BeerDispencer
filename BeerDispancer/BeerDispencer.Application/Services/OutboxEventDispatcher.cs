using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Messaging.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeerDispenser.Application.Services
{
    public class OutboxEventDispatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly NewPaymentPublisher _eventsTrigger;
        private readonly ILogger<OutboxEventDispatcher> _logger;
        private ManualResetEvent _ready;

        public OutboxEventDispatcher(
            IServiceScopeFactory serviceScopeFactory,
            NewPaymentPublisher eventsTrigger,
            ILogger<OutboxEventDispatcher> logger,
            IHostApplicationLifetime lifetime)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _eventsTrigger = eventsTrigger;
            _logger = logger;
           
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IDispencerUof>();

            while (!stoppingToken.IsCancellationRequested)
            { 
                {
                    {
                        var eventsToProcess = await uow.OutboxRepo.GetNotProccessedEvents();

                        foreach (var @event in eventsToProcess)
                        {
                            await ProcessEventAsync(@event, uow, stoppingToken);

                        }
                    }
                   
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
        }

        private async Task ProcessEventAsync(OutboxDto outboxEvent, IDispencerUof uow,  CancellationToken cancellationToken)
        {
            try
            {
                if (outboxEvent.EventType == typeof(EventHolder<NewPaymentEvent>).ToString())
                {
                    await ProcessPaymentEventAsync(outboxEvent, uow, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while executing {method}. Exception: {@ex}", nameof(ProcessEventAsync), ex);
            }
        }

        private async Task ProcessPaymentEventAsync(OutboxDto outboxEvent, IDispencerUof uow, CancellationToken cancellationToken)
        {
            using var transaction = uow.StartTransaction();

            var paymentEvent = JsonConvert.DeserializeObject<EventHolder<NewPaymentEvent>>(outboxEvent.Payload);
            await _eventsTrigger.RaiseEventAsync(paymentEvent, cancellationToken);

            outboxEvent.EventState = EventStateDto.Completed;
            outboxEvent.UpdatedAt = DateTime.UtcNow;
            await uow.OutboxRepo.UpdateAsync(outboxEvent);
            await uow.Complete();
            uow.CommitTransaction();
        }
    }
}

