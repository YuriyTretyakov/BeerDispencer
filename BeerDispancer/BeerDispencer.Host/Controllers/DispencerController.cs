using BeerDispenser.Application.Implementation.Commands;
using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Application.Implementation.Queries;
using BeerDispenser.WebApi.Extensions;
using BeerDispenser.WebApi.ViewModels.Request;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace BeerDispenser.Controllers
{
    [Route("api/[controller]")]
    public class DispenserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DispenserController> _logger;

        public DispenserController(IMediator mediator, ILogger<DispenserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Authorize(Roles = UserRoles.Service)]
        [HttpPost()]
        public async Task<IActionResult> CreateDispencerAsync([FromBody] DispenserCreateCommand createDispencer)
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

        [HttpGet("all")]
        public async Task<IActionResult> GetAllDispencers()
        {
            var query = new GetAllDispensersQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDispencers()
        {
            var query = new GetActiveDispensersQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Service)]
        [HttpPost("/api/Dispenser/{id}/setinactive")]
        public async Task<IActionResult> DeactivateDispenser(Guid id)
        {
            var command = new DeactivateDispenserCommand { Id = id };
            var result = await _mediator.Send(command);

            return result.Result == true ? Ok(result) : BadRequest(result.Details);
        }
    }
}

