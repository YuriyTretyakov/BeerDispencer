using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BeerDispancer.Application.Implementation.Commands.Authorization;
using BeerDispencer.WebApi.ViewModels.Request;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BeerDispencer.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginCommand loginCommand)
        {
            var result = await _mediator.Send(loginCommand);
            return Ok(result.Data);
        }

      

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> logoutAsync()
        {
            var result = await _mediator.Send(new UserLoginCommand());
            return NoContent();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserCommand createUser)
        {
            var result = await _mediator.Send(createUser);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ProblemDetails);
        }
    }
}

