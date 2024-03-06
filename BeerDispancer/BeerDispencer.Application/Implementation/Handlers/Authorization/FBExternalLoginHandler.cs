using BeerDispenser.Application.DTO.Authorization;
using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Shared;
using BeerDispenser.Shared.Dto;
using BeerDispenser.Shared.Dto.ExternalProviders;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
    internal class FBExternalLoginHandler : IRequestHandler<FBExternalLoginCommand, AuthResponseDto>
    {
        private readonly UserManager<CoyoteUser> _userManager;
        private readonly JWTSettings _jwtSettings;
        private readonly JWtTokenProvider _jwtTokenProvider;

        public FBExternalLoginHandler(
            UserManager<CoyoteUser> userManager,
            IOptions<JWTSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _jwtTokenProvider = new JWtTokenProvider(jwtSettings.Value);
        }

        public async Task<AuthResponseDto> Handle(FBExternalLoginCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await GetFbUserDataAsync(request.FbResponse, cancellationToken);

            if (userProfile is null)
            {
                return AuthResponseDto
                     .CreateProblemDetails($"Uable to retrieve FB user profile for user {request.FbResponse.UserId}");
            }

            var user = await _userManager.FindByEmailAsync(userProfile.Email);

            if (user is null)
            {
                user = await CreateInternalUserAsync(userProfile, "Facebook");
            }

            var jwt = _jwtTokenProvider.GenerateToken(user, new[] { Roles.Client });
            return new AuthResponseDto { IsSuccess = true, Data = jwt };
        } 
        private async Task<FBUserProfileDto> GetFbUserDataAsync(FaceBookResponse request, CancellationToken ct)
        {
            using(var httpClient =  new HttpClient()) 
            {
                var baseUrl = "https://graph.facebook.com/me";
                var scope = "email,id,first_name,last_name,name";

                var requestUrl = $"{baseUrl}?fields={scope}&access_token={request.Token}";
                var userDataResponse = await httpClient.GetAsync(requestUrl);

                if (userDataResponse.IsSuccessStatusCode)
                {
                    var userData = JsonConvert.DeserializeObject<FBUserProfileDto>(await userDataResponse.Content.ReadAsStringAsync());
                    return userData;
                }
                
                return null;
       
            }
        }

        private async Task<CoyoteUser> CreateInternalUserAsync(FBUserProfileDto userProfile, string externalProvider)
        {
            var user = new CoyoteUser
            {
                UserName = userProfile.Name?? userProfile.Email,
                Email = userProfile.Email,
                EmailConfirmed = true,
                NormalizedEmail = userProfile.Email.ToUpper(),
                NormalizedUserName = (userProfile?.Name ?? userProfile.Email).ToUpper(),
                LoginProvider = externalProvider,
                PictureUrl = userProfile.PictureUrl
            };
            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, Roles.Client);
            return user;
        }
    }
}
