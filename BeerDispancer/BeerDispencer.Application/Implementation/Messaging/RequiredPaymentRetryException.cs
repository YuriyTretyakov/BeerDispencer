using System;
namespace BeerDispenser.Application.Implementation.Messaging
{
	public class RequiredPaymentRetryException:Exception
	{
		public RequiredPaymentRetryException(string message):base(message)
		{
		}
	}
}

