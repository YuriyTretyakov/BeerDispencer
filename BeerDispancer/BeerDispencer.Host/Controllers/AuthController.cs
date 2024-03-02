using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Application.Implementation.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BeerDispenser.Shared.Dto;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;

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

        [AllowAnonymous]
        [HttpGet("google-signin")]
        public async Task<IActionResult> GoogleSignInCallback(string code)
        {
            var tokenResult = await _mediator.Send(new GoogleLoginCommand { Code = code });

            var response = JsonConvert.SerializeObject(tokenResult);
           
            //return RedirectToAction()

            //HttpContext.ChallengeAsync(Defaultauthentication)
            
            Response.StatusCode = (int)HttpStatusCode.Gone;
            return new JsonResult(response);
        }

        [HttpGet("google-consent-url")]
        public async Task<string> GetGoogleConsentScreenUrl()
        {
            return await _mediator.Send(new GoogleConsentUrlQuery());
        }
    }
}

