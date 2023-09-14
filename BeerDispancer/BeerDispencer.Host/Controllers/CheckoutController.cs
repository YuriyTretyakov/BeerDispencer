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

        public CheckoutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> CheckoutOrder([FromBody] NewOrderDetails orderDetails, [FromServices] IServiceProvider sp)
        {
            var server = sp.GetRequiredService<IServer>();

            var serverAddressesFeature = server.Features.Get<IServerAddressesFeature>();

            string? thisApiUrl = null;

            if (serverAddressesFeature is not null)
            {
                thisApiUrl = serverAddressesFeature.Addresses.FirstOrDefault();
            }

            var dispencerPrePayCommand = new DispencerPrePayCommand
            {
                Amount = orderDetails.Amount,
                Currency = orderDetails.Currency,
                DispencerId = orderDetails.DispencerId,
                WebApiBaseUrl = thisApiUrl,
                WebUiBaseUrl = Request.Headers.Referer[0]

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
        public ActionResult CheckoutSuccess(string sessionId)
        {
            var getPaymentInfo = new GetOrderDetailsQuery { SessionId = sessionId };

            _mediator.Send(getPaymentInfo);
            var webUiUrl = Request.Headers.Referer[0];
            return Redirect(webUiUrl + "success");
        }
    }
}

