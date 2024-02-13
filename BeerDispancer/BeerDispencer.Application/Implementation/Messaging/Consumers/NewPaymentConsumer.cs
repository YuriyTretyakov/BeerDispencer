using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Messaging.Core;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Consumers
{
    public class NewPaymentConsumer : EventConsumerBase<NewPaymentEvent>
	{
		public NewPaymentConsumer(ILogger<EventConsumerBase<NewPaymentEvent>> logger, EventHubConfig configuration)
			: base(logger, configuration)
		{
		}
    }
}

