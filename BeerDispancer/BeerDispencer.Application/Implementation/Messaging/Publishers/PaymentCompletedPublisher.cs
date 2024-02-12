using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Messaging.Core;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Publishers
{
    public class PaymentCompletedPublisher:EventPublisher<PaymentCompletedEvent>
	{
		public PaymentCompletedPublisher(ILogger<PaymentCompletedPublisher> logger, EventHubConfig configuration)
			:base(configuration,  logger)
		{
		}
	}
}

