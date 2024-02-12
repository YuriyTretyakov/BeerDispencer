﻿using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Messaging.Core;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Consumers
{
    public class PaymentToProcessConsumer : EventConsumerBase<PaymentToProcessEvent>
	{
		public PaymentToProcessConsumer(ILogger<PaymentToProcessConsumer> logger, EventHubConfig configuration)
			: base(logger, configuration, nameof(PaymentToProcessConsumer))
		{
		}

        public override string ConfigSectionName => nameof(PaymentToProcessEvent);
    }
}

