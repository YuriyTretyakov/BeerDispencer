using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BeerDispencer.Infrastructure.Implementations;
using BeerDispencer.WebApi.Commands;
using BeerDispencer.WebApi.Extensions;
using BeerDispencer.WebApi.Queries;
using BeerDispencer.WebApi.RequestModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace BeerDispancer.Controllers
{
    [Route("api/[controller]")]
    public class DispencerController : Controller
    {
        private readonly IMediator _mediator;

        public DispencerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDispencerAsync([FromBody] DispencerCreateCommand createDispencer)
        {
            var result = await _mediator.Send(createDispencer);
            return Ok(result);
        }

       
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatusAsync([FromBody] DispenserUpdateModel udpate, Guid id)
        {
            var command = new DispencerUpdateCommand { Id = id, Status = udpate.Status, UpdatedAt = udpate.UpdatedAt };
            var result = await _mediator.Send(command);
            return result == true ? Accepted() : Conflict();
        }
       
     
        [HttpGet("{id}/spending")]
        public async Task<IActionResult> GetSpendingAsync(Guid id)
        {
            var query = new GetSpendingsQuery { DispencerId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

