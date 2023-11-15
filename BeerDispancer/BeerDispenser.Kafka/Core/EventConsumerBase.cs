﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;


namespace BeerDispenser.Kafka.Core
{
    public abstract class EventConsumerBase<T> :/* IDisposable,*/ IEventConsumer<T> where T : class
    {

        IConsumer<string, EventHolder<T>> _consumer;
        private string _topicName;
        CancellationToken _cancellationToken;
        Thread _consumerThread;

        ConcurrentQueue<EventHolder<T>> _messageQueue = new();

        public abstract string ConfigSectionName { get; }

        public EventConsumerBase(KafkaConfig configuration)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration.GetBroker(),
                GroupId = "group1",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            _consumer = new ConsumerBuilder<string, EventHolder<T>>(consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(new DefaultJsonDeserializer<EventHolder<T>>())

                .Build();

            _topicName = configuration.GetTopicName(ConfigSectionName);
           
        }

        private void Consume()
        {
            try
            {
                var consumeResult = _consumer.Consume(_cancellationToken);
                var message = consumeResult.Message?.Value;
                _messageQueue.Enqueue(message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Dispose()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            _cancellationToken = cts.Token;
            _consumer.Close();
            _consumer.Dispose();
        }

        public void StartConsuming(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topicName);

            if (_consumerThread is not null && _consumerThread.ThreadState == ThreadState.Running)
            {
                throw new InvalidOperationException(nameof(EventConsumerBase<T>));
            }

            _cancellationToken = cancellationToken;
            _consumerThread = new Thread(Consume);
            _consumerThread.Start();
        }

        public void Stop(CancellationToken cancellationToken)
        {
            _consumer.Unsubscribe();
            var cts = new CancellationTokenSource();
            cts.Cancel();
            _cancellationToken = cts.Token;
        }

        public IReadonlyEventHolder<T> GetMessages()
        {
            if (_messageQueue.TryDequeue(out var message))
            {
                return message;
            }
            return default;
        }
    }
}

