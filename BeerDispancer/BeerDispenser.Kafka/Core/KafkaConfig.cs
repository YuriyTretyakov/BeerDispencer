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

        public string GetConnectionString()
        {
            return _configuration.GetSection(nameof(KafkaConfig)).Get<KafkaSection>()
                .ConnectionString;
        }

        public string GetEventHubName(string sectionName)
        {
            return _configuration.GetSection(nameof(KafkaConfig)).Get<KafkaSection>()
                .EventHubs[sectionName];
        }
    }


    public class KafkaSection
    {
        public Dictionary<string, string> EventHubs { get; set; }
        public string ConnectionString { get; set; }
    }
}

