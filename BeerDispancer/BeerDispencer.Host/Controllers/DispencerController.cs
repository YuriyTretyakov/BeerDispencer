using BeerDispancer.Application.Implementation.Commands;
using BeerDispancer.Application.Implementation.Commands.Authorization;
using BeerDispancer.Application.Implementation.Queries;
using BeerDispencer.Application.Implementation.Commands;
using BeerDispencer.WebApi.Extensions;
using BeerDispencer.WebApi.ViewModels.Request;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace BeerDispancer.Controllers
{
    [Route("api/[controller]")]
    public class DispencerController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DispencerController> _logger;

        public DispencerController(IMediator mediator, ILogger<DispencerController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Authorize(Roles = UserRoles.Service)]
        [HttpPost()]
        public async Task<IActionResult> CreateDispencerAsync([FromBody] DispencerCreateCommand createDispencer)
        {
            var result = await _mediator.Send(createDispencer);
            return Ok(result.ToViewModel());
        }

       
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatusAsync([FromBody] DispenserUpdateModel update, Guid id)
        {
            var command = update.ToCommand(id);
            var result = await _mediator.Send(command);
            return result.Result ? Accepted() : Conflict();
        }
       
     
        [HttpGet("{id}/spending")]
        public async Task<IActionResult> GetSpendingAsync(Guid id)
        {
            var query = new GetAllSpendingsQuery { DispencerId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllDispencers()
        {
            var query = new GetAllDispencersQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("{id}/processPayment")]
        public async Task<IActionResult> ProcessPaymentAsync([FromBody] CreatePaymentCommand createPayment, Guid id)
        {
            var result = await _mediator.Send(createPayment);
            return Ok(result);
        }
    }
}

