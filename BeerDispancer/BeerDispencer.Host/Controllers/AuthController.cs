using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Application.Implementation.Queries;
using BeerDispenser.Application.Implementation.Commands.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BeerDispenser.Shared.Dto;

namespace BeerDispenser.WebApi.Controllers
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
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ProblemDetails);
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> logoutAsync()
        {
            var result = await _mediator.Send(new LogoutCommand());
            return result.IsSuccess ? NoContent() : BadRequest(result.ProblemDetails); 
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserCommand createUser)
        {
            var result = await _mediator.Send(createUser);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ProblemDetails);
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("getallusers")]
        public async Task<IActionResult> GetAllUsersAsycn()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

       
        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleSignInCallback()
        {
           // var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok();
        }
    }
}

