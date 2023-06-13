using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BeerDispancer.Application.Implementation.Commands.Authorization;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Application.Implementation.Response;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace BeerDispencer.Application.Implementation.Handlers.Authorization
{
	public class LoginUserHandler:IRequestHandler<UserLoginCommand, AuthResponseDto>
	{
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJWTSettings _jwtSettings;

        public LoginUserHandler(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJWTSettings jwtSettings)
		{
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthResponseDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var response = new AuthResponseDto { IsSuccess = true };
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
                return response;

            var roles = await _userManager.GetRolesAsync(user);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, true, false);

            if (!result.Succeeded)
                return response;

            response.Data = GenerateToken(user, roles);

            return response;
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

