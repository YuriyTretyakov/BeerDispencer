using BeerDispencer.Application.Implementation.Commands;
using BeerDispencer.Application.Implementation.Queries;
using BeerDispencer.Shared;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;



namespace BeerDispencer.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CheckoutController : Controller
    {
        private readonly IMediator _mediator;
        private static string _webUiBaseUrl;

        public CheckoutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CheckoutOrder([FromBody] NewOrderDetails orderDetails, [FromServices] IServiceProvider sp)
        {
            var server = sp.GetRequiredService<IServer>();

            var serverAddressesFeature = server.Features.Get<IServerAddressesFeature>();

            string? thisApiUrl = null;

            if (serverAddressesFeature is not null)
            {
                thisApiUrl = serverAddressesFeature.Addresses.FirstOrDefault();
            }

            _webUiBaseUrl = Request.Headers.Referer[0];

            var dispencerPrePayCommand = new DispencerPrePayCommand
            {
                Amount = orderDetails.Amount,
                Currency = orderDetails.Currency,
                DispencerId = orderDetails.DispencerId,
                WebApiBaseUrl = thisApiUrl,
                WebUiBaseUrl = _webUiBaseUrl
            };


            var checkoutOrderResponse = await _mediator.Send(dispencerPrePayCommand);

            if (checkoutOrderResponse is not null)
            {
                return Ok(checkoutOrderResponse);
            }

            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("success")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult CheckoutSuccess(string sessionId)
        {
            var getPaymentInfo = new GetOrderDetailsQuery { SessionId = sessionId };

            _mediator.Send(getPaymentInfo);
            return Redirect(_webUiBaseUrl + "bar");
        }
    }
}

