using Confluent.Kafka;
using Newtonsoft.Json;

namespace BeerDispenser.Kafka.Core
{
    public class Producer<T> : IProducer<T> where T : class
    {
        private string _broker { get; set; }

        private IProducer<string, string> _kafkaProducer;
        private JsonSerializerSettings _settings;


        public Producer(KafkaConfig kafkaConfig)
        {
            _broker = kafkaConfig.GetBroker();

            var config = new ProducerConfig
            {
                BootstrapServers = _broker
            };

            _kafkaProducer = new ProducerBuilder<string, string>(config).Build();
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

            await _kafkaProducer
                .ProduceAsync(topicName, kafkaMessage, cancellationToken);
        }

        public void Dispose()
        {
            _kafkaProducer.Dispose();
        }
    }
}

