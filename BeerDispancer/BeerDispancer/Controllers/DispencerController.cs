using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeerDispancer.ApplicationLayer;
using BeerDispancer.Extensions;
using BeerDispancer.PresentationLayer;
using Microsoft.AspNetCore.Mvc;



namespace BeerDispancer.Controllers
{
    [Route("api/[controller]")]
    public class DispencerController : Controller
    {
        private readonly DispencerManager _dispencerManager;

        public DispencerController(DispencerManager dispencerManager)
        {
            _dispencerManager = dispencerManager;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDispancer([FromBody] DispencerCreate createDispancer)
        {
            var dispencerDto = createDispancer.ToDto();

            var dispencer = await _dispencerManager.CreateDispencerAsync(dispencerDto);
            var dispencerResponse = dispencer.ToViewModel();

            return Ok(dispencerResponse);
        }

       
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatusAsync([FromBody] DispenserUpdate udpateCommand, Guid id)
        {
            var updateDt = udpateCommand.ToDto(id);
            var result = await _dispencerManager.ChangeDispancerStateAsync(updateDt);
            return result == true ? Accepted() : Conflict();
        }
       
     
        [HttpGet("{id}/spending")]
        public IActionResult GetSpending(Guid id)
        {
            var result =  _dispencerManager.GetSpending(id);
            return Ok(result);
        }
    }
}

