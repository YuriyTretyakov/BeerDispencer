using System;
using BeerDispancer.Application.Implementation.Commands.Authorization;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Application.Implementation.Commands.Authorization;
using BeerDispencer.Application.Implementation.Response;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BeerDispencer.Application.Implementation.Handlers.Authorization
{
	public class LogoutUserHandler:IRequestHandler<LogoutCommand, AuthResponseDto>
	{
        private readonly ITokenManager _tokenManager;

        public LogoutUserHandler(ITokenManager tokenManager)
		{
            _tokenManager = tokenManager;
        }

        public async Task<AuthResponseDto> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _tokenManager.DeactivateCurrentAsync();
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

