using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Kafka.Core
{
    public abstract class EventConsumerBase<T> :IDisposable where T : class
    {
        IConsumer<string, EventHolder<T>> _consumer;
        private string _topicName;
        private readonly ILogger _logger;

        public abstract string ConfigSectionName { get; }

        public class NewMessageEvent : EventArgs
        {
            public ConsumeResult<string, EventHolder<T>>  Event { get; init; }
        }

        public delegate void NotifyNewMessage(object sender, NewMessageEvent e);

        public event NotifyNewMessage OnNewMessage;

        public EventConsumerBase(ILogger logger, KafkaConfig configuration, string consumerId)
        {
            _logger = logger;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration.GetBroker(),
                GroupId = consumerId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                Debug = "broker, consumer",
                ReconnectBackoffMs = 1000,
                ReconnectBackoffMaxMs = 60 * 1000,
                MessageMaxBytes = 1000,
                EnableAutoCommit = false
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

        private void FireNewMessageEvent(ConsumeResult<string, EventHolder<T>> consumeResult )
        {
            OnNewMessage?.Invoke(this, new NewMessageEvent { Event = consumeResult });
        }

        public void Commit(ConsumeResult<string, EventHolder<T>> consumeResult)
        {
            _consumer.Commit(consumeResult);
        }

        private void ConsumeAsync(CancellationToken cancellationToken)
        {
           _= Task.Factory.StartNew(() =>
           {
               while (!cancellationToken.IsCancellationRequested)
               {
                   var consumeResult = _consumer.Consume(1000);
                   //var message = consumeResult?.Message?.Value;

                   if (consumeResult is null)
                   {
                       continue;
                   }

                   _logger.LogInformation("Consumer {name} message received: {@message} Offset: {offset}",
                       _topicName,
                       consumeResult?.Message?.Value,
                       consumeResult.Offset);
                   FireNewMessageEvent(consumeResult);
               }
           },
           TaskCreationOptions.LongRunning
           ).ConfigureAwait(false);
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
            ConsumeAsync(cancellationToken);
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

