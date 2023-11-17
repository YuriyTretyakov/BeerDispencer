using System;
using System.Collections.Concurrent;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using static Confluent.Kafka.ConfigPropertyNames;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Kafka.Core
{
    public abstract class EventConsumerBase<T> :IDisposable, IEventConsumer<T> where T : class
    {

        IConsumer<string, EventHolder<T>> _consumer;
        private string _topicName;
        CancellationToken _cancellationToken;
        Task _consumerTask;

        ConcurrentQueue<EventHolder<T>> _messageQueue = new();
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
               // EnableAutoCommit = false,
               // EnableAutoOffsetStore = true,
                //MessageMaxBytes = 1024,
                //MaxPollIntervalMs = 86400000,
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

        private void Consume()
        {
            try
            {
                var consumeResult = _consumer.Consume(_cancellationToken);
                var message = consumeResult.Message?.Value;
                _logger.LogInformation("Consumer {name} message received: {@message}", _topicName, message);
                _messageQueue.Enqueue(message);
                _consumer.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError("Consuming error for topic name: {name}. Exception: {@ex}", _topicName, ex);
                throw;
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("Consumer built for topic name: {name} dispose called", _topicName);
            _consumer.Close();
            _consumer.Dispose();
        }

        public void StartConsuming(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _consumer.Subscribe(_topicName);

            if (_consumerTask is not null && _consumerTask.Status == TaskStatus.Running)
            {
                _logger.LogError("Consuming already started for topic name: {name}", _topicName);
                throw new InvalidOperationException(nameof(EventConsumerBase<T>));
            }

            _consumerTask = Task
                .Factory
                .StartNew(
                Consume,
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            _logger.LogInformation("Consuming started {name}", _topicName);
        }

        public void Stop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consuming stopped {name}", _topicName);
            _consumer.Unsubscribe();
            _consumer.Close();
            _consumer.Dispose();
        }

        public EventHolder<T> GetMessages()
        {
            try
            {
                if (_messageQueue.TryDequeue(out var message))
                {
                    return message;
                }
                return default;
            }
            finally
            {
                Thread.Yield();
            }

        }
    }
}

