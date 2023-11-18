using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Kafka.Core
{
    public abstract class EventConsumerBase<T> : IEventConsumer<T> where T : class
    {

        IConsumer<string, EventHolder<T>> _consumer;
        private string _topicName;
        private readonly ILogger _logger;

        public abstract string ConfigSectionName { get; }

        public EventConsumerBase(ILogger logger, KafkaConfig configuration)
        {
            _logger = logger;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration.GetBroker(),
                GroupId = "group1",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            _logger.LogInformation("{name}: {@consumerConfig}", nameof(ConsumerConfig), consumerConfig);

            _consumer = new ConsumerBuilder<string, EventHolder<T>>(consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(new DefaultJsonDeserializer<EventHolder<T>>())
                .SetErrorHandler(OnConsumerError)
                .Build();

            _topicName = configuration.GetTopicName(ConfigSectionName);
            _logger.LogInformation("Consumer built for {eventholder} topic name: {name}", typeof(EventHolder<T>), _topicName);
        }

        private void OnConsumerError(IConsumer<string, EventHolder<T>> consumer, Error error)
        {
            _logger.LogError("Consumer {name} error: {@error}", _topicName, error);
        }

        public async Task<EventHolder<T>> ConsumeAsync(CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                var message = consumeResult?.Message?.Value;
               
                _consumer.Commit(consumeResult);
                _logger.LogInformation("Consumer {name} message received: {@message} Offset: {offset}",
                    _topicName,
                    message,
                    consumeResult.Offset);
                return message;
            }, cancellationToken);
        }

        public void Dispose()
        {
            _logger.LogInformation("Consumer built for topic name: {name} dispose called", _topicName);
            _consumer.Close();
            _consumer.Dispose();
        }

        public void StartConsuming(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topicName);
            _logger.LogInformation("Consuming started {name}", _topicName);
        }

        public void Stop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consuming stopped {name}", _topicName);
            _consumer.Unsubscribe();
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}

