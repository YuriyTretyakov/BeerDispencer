using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Messaging.Core;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Publishers
{
    public class PaymentToProcessPublisher : EventPublisher<PaymentInProccessEvent>
	{
		public PaymentToProcessPublisher(ILogger<PaymentToProcessPublisher> logger, EventHubConfig configuration)
			:base(configuration, logger)
		{
		}
	}
}

