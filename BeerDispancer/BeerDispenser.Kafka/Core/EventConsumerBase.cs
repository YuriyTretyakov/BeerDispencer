using System.Text;
using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeerDispenser.Messaging.Core
{
    public abstract class EventConsumerBase<T> where T : class
    {
        private string _eventHubName;
        private string _connectionString;
        private EventHubConsumerClient _consumerClient;

        private readonly ILogger<EventConsumerBase<T>> _logger;
        private readonly string _consumerId;

        public class NewMessageEvent : EventArgs
        {
            public EventHolder<T> Event { get; init; }
        }

        public delegate void NotifyNewMessage(object sender, NewMessageEvent e);

        public event NotifyNewMessage OnNewMessage;

        protected EventConsumerBase(ILogger<EventConsumerBase<T>> logger, EventHubConfig configuration)
        {
            _logger = logger;
            //_consumerId = consumerId;
            _eventHubName = configuration.GetEventHubName(typeof(T).Name);
            _connectionString = configuration.GetConnectionString();
        }

        public Task Start(CancellationToken cancellationToken)
        {
            _consumerClient = new EventHubConsumerClient(
                EventHubConsumerClient.DefaultConsumerGroupName,
                _connectionString,
                _eventHubName);

            return Task.Factory.StartNew(x => StartConsumingAsync(cancellationToken), TaskCreationOptions.LongRunning);
        }

        private void FireNewMessageEvent(EventHolder<T> consumeResult)
        {
            _logger.LogInformation("EventHub Consumer [{hub}]: New message consumed: {@message}", typeof(EventHolder<T>).Name, consumeResult);
            OnNewMessage?.Invoke(this, new NewMessageEvent { Event = consumeResult });
        }

        private async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("EventHub Consumer [{eventholder}]:  name '{name}' - Consuming started", typeof(EventHolder<T>).Name, _eventHubName);
            var ro = new ReadEventOptions { MaximumWaitTime = TimeSpan.FromSeconds(1), PrefetchCount = 1 };
            while (!cancellationToken.IsCancellationRequested)
            {

                await foreach (var partitionEvent in _consumerClient.ReadEventsAsync(
                       ro, cancellationToken))
                {
                    //if (partitionEvent.Data is null)
                    //{
                    //    _logger.LogInformation("Consumer {eventholder} No Data found", typeof(EventHolder<T>));
                    //}
                    //else
                    //{
                        var json = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());

                        var @event = JsonConvert.DeserializeObject<EventHolder<T>>(json);
                        FireNewMessageEvent(@event);
                    //}
                }
            }
        }

        public async Task Stop(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("EventHub Consumer [{eventholder}]: Stopped.", _eventHubName);
            await _consumerClient.CloseAsync();
            await _consumerClient.DisposeAsync();
        }
    }
}