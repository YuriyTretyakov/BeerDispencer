using BeerDispenser.Application.Implementation.Commands;
using BeerDispenser.Shared;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace BeerDispenser.Application.Implementation.Handlers
{
    public class DispenserPaymentHandler:IRequestHandler<DispenserPrePayCommand, CheckoutOrderResponse>
    {
        private readonly IConfiguration _configuration;

        public DispenserPaymentHandler(IConfiguration configuration)
		{
            _configuration = configuration;
        }

        public async Task<CheckoutOrderResponse> Handle(DispenserPrePayCommand request, CancellationToken cancellationToken)
        {

            var sessionId = await CheckOutAsync(request, request.WebApiBaseUrl, request.WebUiBaseUrl);

            if (sessionId is not null)
            {
                var pubKey = _configuration["Stripe:PubKey"];

                var checkoutOrderResponse = new CheckoutOrderResponse()
                {
                    SessionId = sessionId,
                    PubKey = pubKey
                };

                return checkoutOrderResponse;
            }


            return null;

        }

        public async Task<string> CheckOutAsync(DispenserPrePayCommand orderDetails, string thisApiUrl, string wasmUrl)
        {
            var metadata = new Dictionary<string, string>
            {
                {nameof(OrderResponseDetails.ProductId ), orderDetails.DispenserId.ToString() }
            };

            var options = new SessionCreateOptions
            {

                SuccessUrl = $"{thisApiUrl}/api/checkout/success?sessionId=" + "{CHECKOUT_SESSION_ID}",
                CancelUrl = wasmUrl + "failed",
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = decimal.ToInt64(orderDetails.Amount*100),
                        Currency = orderDetails.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = orderDetails.DispenserId.ToString(),
                            Description = $"Pre-payment for usage of dispencer {orderDetails.DispenserId}",
                            Images = new List<string>{ "https://she.hr/wp-content/uploads/2014/09/piv.jpg" }
                        },
                    },
                    Quantity = 1,
                },
            },
                Mode = "payment",
                Metadata = metadata
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Id;
        }
    }
}

