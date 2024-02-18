using System.Text;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeerDispenser.Messaging.Core
{
    public abstract class EventConsumerBase<T> where T : class
    {
        private string _eventHubName;
        private string _connectionString;
        private EventHubConsumerClient _consumerClient;

        private readonly ILogger _logger;

        protected abstract Task OnNewMessage(IReadonlyEventHolder<T> message, CancellationToken cancellationToken);

        protected EventConsumerBase(ILogger logger, EventHubConfig configuration)
        {
            _logger = logger;
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

        private async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            LogInfo("Consuming started");
            var ro = new ReadEventOptions { MaximumWaitTime = TimeSpan.FromSeconds(10), PrefetchCount = 1, TrackLastEnqueuedEventProperties = true };
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await foreach (var partitionEvent in _consumerClient.ReadEventsAsync(
                           ro,
                           cancellationToken))
                    {
                        if (partitionEvent.Data is not null)
                        {
                            var json = Encoding.UTF8.GetString(partitionEvent.Data.EventBody.ToArray());
                            var @event = JsonConvert.DeserializeObject<EventHolder<T>>(json);
                           
                            LogInfo("Message Received: {@event} Offset: {offset} EnqueuedTime: {time} Partition: {partition}",
                            @event,
                            partitionEvent.Data.Offset,
                            partitionEvent.Data.EnqueuedTime,
                            partitionEvent.Partition.PartitionId);
                            await OnNewMessage(@event, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            LogInfo("Heartbeat message");
                        }
                    }
                }
                catch(Exception e)
                {
                    LogError("Error while consuming: {@err}", e);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            LogInfo("Stopping...");
            await _consumerClient.CloseAsync(cancellationToken);
            LogInfo("ConsumerClient closed.");
            await _consumerClient.DisposeAsync();
            LogInfo("ConsumerClient disposed.");
        }

        protected void LogInfo(string? message, params object?[] args)
        {
            var messagePrefix = $"EventHub Consumer [{_eventHubName}]: ";
            message = messagePrefix + message;
            _logger.LogInformation(message, args);
        }

        protected void LogError(string? message, params object?[] args)
        {
            var messagePrefix = $"EventHub Consumer [{_eventHubName}]: ";
            message = messagePrefix + message;
            _logger.LogError(message, args);
        }
    }
}