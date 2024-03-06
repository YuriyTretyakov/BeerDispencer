using BeerDispenser.Application.DTO.Authorization;
using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
    public class LoginUserHandler:IRequestHandler<UserLoginCommand, AuthResponseDto>
	{
        private readonly UserManager<CoyoteUser> _userManager;
        private readonly SignInManager<CoyoteUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly JWtTokenProvider _jwtTokenProvider;

        public LoginUserHandler(
            UserManager<CoyoteUser> userManager,
            SignInManager<CoyoteUser> signInManager,
            IOptions<JWTSettings> jwtSettings)
		{
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;

            _jwtTokenProvider = new JWtTokenProvider(jwtSettings.Value);
        }

        public async Task<AuthResponseDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
           
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return AuthResponseDto.CreateProblemDetails($"Failed to login: User not found '{request.UserName}'");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, true, false);

            if (!result.Succeeded)
            {
                return AuthResponseDto
                    .CreateProblemDetails($"Failed to login: login credentials are invalid");
            }

            return new AuthResponseDto
            {
                IsSuccess = true,
                Data = _jwtTokenProvider.GenerateToken(user, roles)
            };
        }
    }
}

