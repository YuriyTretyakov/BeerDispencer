using System;
using BeerDispancer.Application.Implementation.Commands.Authorization;
using BeerDispencer.Application.Implementation.Response;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BeerDispencer.Application.Implementation.Handlers.Authorization
{
	public class LogoutUserHandler:IRequestHandler<UserLoginCommand, AuthResponseDto>
	{
        private readonly SignInManager<IdentityUser> _signInManager;

        public LogoutUserHandler(SignInManager<IdentityUser> signInManager)
		{
            _signInManager = signInManager;
        }

        public async Task<AuthResponseDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _signInManager.SignOutAsync();
                return new AuthResponseDto { IsSuccess = true };
            }
            catch(Exception e)
            {
                return new AuthResponseDto { IsSuccess = false,
                    ProblemDetails = new[]
                    { new AuthDetails { Code = e.Message } } };
            } 
        }
    }
}

