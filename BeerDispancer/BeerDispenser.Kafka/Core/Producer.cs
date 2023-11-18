using System.Diagnostics;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeerDispenser.Kafka.Core
{
    public class Producer<T> : IProducer<T> where T : class
    {
        private string _broker { get; set; }

        private IProducer<string, string> _kafkaProducer;
        private JsonSerializerSettings _settings;
        private readonly ILogger _logger;

        public Producer(KafkaConfig kafkaConfig, ILogger logger)
        {
            _broker = kafkaConfig.GetBroker();

            var config = new ProducerConfig
            {
                BootstrapServers = _broker,
                Acks =Acks.Leader
            };

            _kafkaProducer = new ProducerBuilder<string, string>(config).Build();
            _logger = logger;
        }

        public async Task ProduceAsync(
            string topicName,
            IReadonlyEventHolder<T> @event,
            CancellationToken cancellationToken)
        {
            var messageJson = JsonConvert.SerializeObject(@event, _settings);

            var kafkaMessage = new Message<string, string>
            {
                Key = @event.Key.ToString(),
                Value = messageJson
            };
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = await _kafkaProducer
                .ProduceAsync(topicName, kafkaMessage, cancellationToken);

            stopWatch.Stop();

            _logger.LogInformation(
                "Producer {topicName} producing message: {@event} Offset: {offset} Duration: {time}",
                topicName,
                @event,
                result.Offset,
                stopWatch.Elapsed);
        }

        public void Dispose()
        {
            _logger.LogInformation("Producer {type} Dispose initiated", typeof(T));
            _kafkaProducer.Dispose();
        }
    }
}

