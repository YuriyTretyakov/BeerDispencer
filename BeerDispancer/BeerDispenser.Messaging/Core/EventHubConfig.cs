using Microsoft.Extensions.Configuration;

namespace BeerDispenser.Messaging.Core
{
    public class EventHubConfig
    {
        private readonly IConfiguration _configuration;

        public EventHubConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetSection(nameof(EventHubConfig)).Get<EventHubSection>()
                .ConnectionString;
        }

        public string GetEventHubName(string sectionName)
        {
            return _configuration.GetSection(nameof(EventHubConfig)).Get<EventHubSection>()
                .EventHubs[sectionName];
        }
    }


    public class EventHubSection
    {
        public Dictionary<string, string> EventHubs { get; set; }
        public string ConnectionString { get; set; }
    }
}

