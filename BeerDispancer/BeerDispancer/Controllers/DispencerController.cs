using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beerdispancer.Infrastructure.DTO;
using BeerDispencer.Infrastructure.Implementations;
using BeerDispencer.WebApi.Commands;
using BeerDispencer.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;



namespace BeerDispancer.Controllers
{
    [Route("api/[controller]")]
    public class DispencerController : Controller
    {
        private readonly DispencerService _dispencerService;

        public DispencerController(DispencerService dispencerMService)
        {
            _dispencerService = dispencerMService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDispancer([FromBody] DispencerCreateCommand createDispancer)
        {
            var dispencerDto = createDispancer.ToDto();
            var dispencer = await _dispencerService.CreateDispencerAsync(dispencerDto);
            var dispencerResponse = dispencer.ToViewModel();

            return Ok(dispencerResponse);
        }

       
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatusAsync([FromBody] DispenserUpdateCommand udpateCommand, Guid id)
        {
            var updateDto = udpateCommand.ToDto(id);
            var result = await _dispencerService.ChangeDispancerStateAsync(updateDto);
            return result == true ? Accepted() : Conflict();
        }
       
     
        [HttpGet("{id}/spending")]
        public IActionResult GetSpending(Guid id)
        {
            var result =  _dispencerService.GetSpending(id);
            return Ok(result.ToViewModel());
        }
    }
}

