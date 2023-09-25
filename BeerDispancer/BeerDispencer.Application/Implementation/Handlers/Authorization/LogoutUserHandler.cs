using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Response;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
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

