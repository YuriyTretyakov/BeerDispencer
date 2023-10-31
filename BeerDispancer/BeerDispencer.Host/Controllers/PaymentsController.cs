using BeerDispenser.Application.Implementation.Commands;
using BeerDispenser.Application.Implementation.Queries;
using BeerDispenser.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BeerDispenser.Application.Implementation.Commands.Payments;

namespace BeerDispenser.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly IMediator _mediator;
        private static string _webUiBaseUrl;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize()]
        [HttpPost("addcard")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> AddCard([FromBody] StripeTokenResponse tokenResponse)
        {
            var claim = User.Claims.First(x => x.Type == "Id");
            var id = Guid.Parse(claim.Value);

            var command = new AddPaymentDetailsCommand
            {
                StripeData = tokenResponse,
                UserId = id
            };

            var result = await _mediator.Send(command);

            return Ok();
        }

        [Authorize()]
        [HttpGet("getcards")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetCards()
        {
            var claim = User.Claims.First(x => x.Type == "Id");
            var id = Guid.Parse(claim.Value);

            var command = new GetUserCardsQuery
            {
                UserId = id
            };

            var result = await _mediator.Send(command);

            var response = result.Select(x =>
            {
                return new PaymentCardViewModel
                {
                    Id = x.Id,
                    IsDefault = x.IsDefault,
                    City = x.City,
                    AdressCountry = x.AdressCountry,
                    Line1 = x.Line1,
                    State = x.State,
                    Zip = x.Zip,
                    Brand = x.Brand,
                    Country = x.Country,
                    CvcCheck = x.CvcCheck,
                    ExpMonth = x.ExpMonth,
                    ExpYear = x.ExpYear,
                    Last4 = x.Last4,
                    AccountHolderName = x.AccountHolderName,
                    Created = x.Created,
                    Email = x.Email
                };
            });

            return Ok(response);
        }

        [Authorize()]
        [HttpDelete("{cardId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> DeleteCard(Guid cardId)
        {
            var claim = User.Claims.First(x => x.Type == "Id");
            var id = Guid.Parse(claim.Value);

            var command = new DeleteCardCommand
            {
                UserId = id,
                CardId = cardId
            };

            var result = await _mediator.Send(command);
            return NoContent();
        }

        [Authorize()]
        [HttpGet("{cardId}/setdefault")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> SetDefaultCard(Guid cardId)
        {
            var claim = User.Claims.First(x => x.Type == "Id");
            var id = Guid.Parse(claim.Value);

            var command = new SetDefaultCardCommand
            {
                UserId = id,
                CardId = cardId
            };

            var result = await _mediator.Send(command);
            return NoContent();
        }

    }
}