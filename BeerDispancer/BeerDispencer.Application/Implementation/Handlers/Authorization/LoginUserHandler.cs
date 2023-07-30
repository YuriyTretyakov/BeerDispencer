using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BeerDispancer.Application.Implementation.Commands.Authorization;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Application.Implementation.Response;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace BeerDispencer.Application.Implementation.Handlers.Authorization
{
	public class LoginUserHandler:IRequestHandler<UserLoginCommand, AuthResponseDto>
	{
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JWTSettings _jwtSettings;

        public LoginUserHandler(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptions<JWTSettings> jwtSettings)
		{
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
           
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    ProblemDetails = new List<AuthDetails> { new AuthDetails { Description = "Failed to login" } }.ToArray()
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, true, false);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    ProblemDetails = new List<AuthDetails> { new AuthDetails { Description = "Failed to login" } }.ToArray()
                };  
            }

            return new AuthResponseDto
            {
                IsSuccess = true,
                Data = GenerateToken(user, roles)
            };
        }

        private string GenerateToken(IdentityUser user, IList<string> userClaims)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>(userClaims.Select(x => new Claim(ClaimTypes.Role, x)))
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName)
            };


            var isAdmin = userClaims.Contains(UserRoles.Admin);

            var token = new JwtSecurityToken(_jwtSettings.Audience,
                _jwtSettings.Issuer,
                claims,
                expires: isAdmin ? DateTime.Now.AddMinutes(15) : DateTime.Now.AddDays(1),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}

