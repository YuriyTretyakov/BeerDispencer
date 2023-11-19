using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Kafka.Core
{
    public abstract class EventPublisher<T> : IDisposable where T : class
    {
        private readonly Producer<T> _producer;

        public string ConfigSectionName { get; private set; }

        public EventPublisher(KafkaConfig configuration, string topicSectionName, ILogger logger)
        {
            ConfigSectionName = configuration.GetTopicName(topicSectionName);
            _producer = new Producer<T>(configuration, logger);
        }

        public async Task RaiseEventAsync(IReadonlyEventHolder<T> @event, CancellationToken cancellationToken)
        {
            await _producer
                .ProduceAsync(ConfigSectionName, @event, cancellationToken);
        }

        public async Task RetryAsync(IReadonlyEventHolder<T> @event, CancellationToken cancellationToken)
        {
            @event.IncrementRetries();
            await _producer
                .ProduceAsync(ConfigSectionName, @event, cancellationToken);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}

