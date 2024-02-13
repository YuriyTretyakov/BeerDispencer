using System;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Messaging.Core;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Publishers
{
    public class NewPaymentPublisher : EventPublisher<NewPaymentEvent>
    {
        public NewPaymentPublisher(ILogger<NewPaymentEvent> logger, EventHubConfig configuration)
            : base(configuration, logger)
        {
        }
    }
}

