using System.Text;
using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeerDispenser.Kafka.Core
{
    public abstract class EventConsumerBase<T>:IDisposable where T : class
    {
        private string _eventHubName;
        private string _connectionString;
        private EventHubConsumerClient _consumerClient;
        
        private readonly ILogger _logger;
        private readonly string _consumerId;

        public abstract string ConfigSectionName { get; }

        public class NewMessageEvent : EventArgs
        {
            public  EventHolder<T>  Event { get; init; }
        }

        public delegate void NotifyNewMessage(object sender, NewMessageEvent e);

        public event NotifyNewMessage OnNewMessage;

        protected EventConsumerBase(ILogger logger, KafkaConfig configuration, string consumerId)
        {
            _logger = logger;
            _consumerId = consumerId;
            _eventHubName = configuration.GetEventHubName(ConfigSectionName);
            _connectionString = configuration.GetConnectionString();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consuming Started {name}", _eventHubName);

            _consumerClient = new EventHubConsumerClient(_consumerId, _connectionString, _eventHubName);

           

            _= Task.Factory.StartNew(x => StartConsumingAsync(cancellationToken), TaskCreationOptions.LongRunning);
        }

        private void FireNewMessageEvent(EventHolder<T> consumeResult )
        {
            OnNewMessage?.Invoke(this, new NewMessageEvent { Event = consumeResult });
        }

        public void Dispose()
        {
            _logger.LogInformation("Consumer built for topic name: {name} dispose called", _eventHubName);
            _consumerClient?.CloseAsync().GetAwaiter().GetResult();
            _consumerClient?.DisposeAsync().GetAwaiter().GetResult();
        }

        private async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            EventPosition startingPosition = EventPosition.Earliest;

            var ro = new ReadEventOptions { MaximumWaitTime = TimeSpan.FromSeconds(1), PrefetchCount = 1 };
            while (!cancellationToken.IsCancellationRequested)
            {

                await foreach (var partitionEvent in _consumerClient.ReadEventsAsync(
                       ro, cancellationToken))
                {
                    if (partitionEvent.Data is null)
                    {
                        _logger.LogInformation("Consumer {eventholder} No Data found", typeof(EventHolder<T>));
                    }
                    else
                    {
                        var json = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());

                        _logger.LogInformation("Consumer built for {eventholder} topic name: {name}", typeof(EventHolder<T>), _eventHubName);

                        var @event = JsonConvert.DeserializeObject<EventHolder<T>>(json);
                        FireNewMessageEvent(@event);
                    }
                }
                }
        }

        public Task Stop(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consuming stopped {name}", _eventHubName);

            Dispose();
            return Task.CompletedTask;
        }
    }
}

