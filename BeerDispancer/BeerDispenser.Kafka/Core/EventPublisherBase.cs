using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Kafka.Core
{
    public abstract class EventPublisherBase<T> : IEventPublisher<T> where T : class
    {
        private readonly IProducer<T> _producer;
        private readonly ILogger _logger;

        public string ConfigSectionName { get; private set; }

        public EventPublisherBase(KafkaConfig configuration, string topicSectionName, ILogger logger)
        {
            ConfigSectionName = configuration.GetTopicName(topicSectionName);
            _producer = new Producer<T>(configuration, logger);
            _logger = logger;
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
    }
}

