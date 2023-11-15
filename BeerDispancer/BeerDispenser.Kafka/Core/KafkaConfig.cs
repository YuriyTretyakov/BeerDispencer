using Microsoft.Extensions.Configuration;

namespace BeerDispenser.Kafka.Core
{
    public class KafkaConfig
    {
        private readonly IConfiguration _configuration;

        public KafkaConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetBroker()
        {
            return _configuration.GetSection(nameof(KafkaConfig)).Get<KafkaSection>()
                .Broker;
        }

        public string GetTopicName(string sectionName)
        {
            return _configuration.GetSection(nameof(KafkaConfig)).Get<KafkaSection>()
                .Topics[sectionName];
        }
    }


    public class KafkaSection
    {
        public Dictionary<string, string> Topics { get; set; }
        public string Broker { get; set; }
    }
}

