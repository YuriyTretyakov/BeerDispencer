using BeerDispenser.Application.Implementation.Commands.Authorization;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using BeerDispenser.Shared.Dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using BeerDispenser.Shared;
using BeerDispenser.Application.DTO.Authorization;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
    internal class GoogleExternalLoginHandler : IRequestHandler<GoogleExternalLoginCommand, AuthResponseDto>
    {
        private readonly UserManager<CoyoteUser> _userManager;
        private readonly JWTSettings _jwtSettings;
        private readonly JWtTokenProvider _jwtTokenProvider;

        public GoogleExternalLoginHandler(
            UserManager<CoyoteUser> userManager,
            IOptions<JWTSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _jwtTokenProvider = new JWtTokenProvider(jwtSettings.Value);
        }

        public async Task<AuthResponseDto> Handle(GoogleExternalLoginCommand request, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.CanReadToken(request.GoogleJwt))
            {
                var jwtSecurityToken = tokenHandler.ReadJwtToken(request.GoogleJwt);
                var userProfile = ParseGoogleJwtToken(jwtSecurityToken);

                var user = await _userManager.FindByEmailAsync(userProfile.Email);

                if (user is null)
                {
                    user = await CreateInternalUserAsync(userProfile, "Google");
                }

                var jwt = _jwtTokenProvider.GenerateToken(user, new[] { Roles.Client });
                return new AuthResponseDto { IsSuccess = true, Data = jwt };
            }

            return  AuthResponseDto.CreateProblemDetails("Unable to read Google JWT token");
        }
         private GoogleUserProfileDto ParseGoogleJwtToken(JwtSecurityToken googleToken)
        {
            var prictureRaw = googleToken.Claims.FirstOrDefault(c => c.Type == "picture").Value;
            if (prictureRaw != null) 
            {
                prictureRaw= prictureRaw.Split("=").FirstOrDefault();
            }

            return new GoogleUserProfileDto
            {
                Email = googleToken.Claims.First(c => c.Type == "email").Value,
                VerifiedEmail = bool.Parse(googleToken.Claims.First(c => c.Type == "email_verified").Value),
                Name = googleToken.Claims.First(c => c.Type == "name").Value,
                GivenName = googleToken.Claims.First(c => c.Type == "given_name").Value,
                FamilyName = googleToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
                Locale = googleToken.Claims.First(c => c.Type == "locale").Value,
                Picture = prictureRaw;
            };
        }

        private async Task<CoyoteUser> CreateInternalUserAsync(GoogleUserProfileDto userProfile, string externalProvider)
        {
            var user = new CoyoteUser
            {
                UserName = userProfile?.GivenName?? userProfile.Email,
                Email = userProfile.Email,
                EmailConfirmed = userProfile.VerifiedEmail,
                NormalizedEmail = userProfile.Email.ToUpper(),
                NormalizedUserName = (userProfile?.GivenName ?? userProfile.Email).ToUpper(),
                LoginProvider = externalProvider,
                PictureUrl =userProfile.Picture?.Split('=')?.FirstOrDefault()
            };
            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, Roles.Client);
            return user;
        }
    }
}
