using System.Diagnostics;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeerDispenser.Kafka.Core
{
    public class Producer<T>:IDisposable  where T : class
    {
        private string _connectionString { get; set; }

        private JsonSerializerSettings _settings;
        private readonly ILogger _logger;
        private readonly string _eventHubName;
        private readonly EventHubProducerClient _producerClient;

        public Producer(KafkaConfig kafkaConfig, ILogger logger)
        {
            _connectionString = kafkaConfig.GetConnectionString();
            _logger = logger;
            _eventHubName = kafkaConfig.GetEventHubName(typeof(T).Name);

            _producerClient = new EventHubProducerClient(_connectionString, _eventHubName);
        }

        public async Task ProduceAsync(
            IReadonlyEventHolder<T> @event,
            CancellationToken cancellationToken)
        {
            var messageJson = JsonConvert.SerializeObject(@event, _settings);

            using var eventBatch = await _producerClient.CreateBatchAsync();
            
            var eventData = new EventData(Encoding.UTF8.GetBytes(messageJson));
            eventBatch.TryAdd(eventData);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            await _producerClient.SendAsync(eventBatch, cancellationToken);
            stopWatch.Stop();

            _logger.LogInformation(
                "Producer {topicName} producing message: {@event}  Duration: {time}",
                _eventHubName,
                @event,
                stopWatch.Elapsed);
        }

        public void Dispose()
        {
            _producerClient.CloseAsync().GetAwaiter().GetResult();
            _producerClient.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}

