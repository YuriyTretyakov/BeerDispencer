using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Shared.Dto.ExternalProviders.Google;
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
using BeerDispenser.Application.DTO;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
    internal class GoogleExternalLoginHandler : IRequestHandler<GoogleExternalLoginCommand, AuthResponseDto>
    {
        private readonly UserManager<CoyoteUser> _userManager;
        private readonly JWTSettings _jwtSettings;

        public GoogleExternalLoginHandler(
            UserManager<CoyoteUser> userManager,
            IOptions<JWTSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
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
                    user = await CreateExternalUserAsync(userProfile, "Google");
                }

                var picture = userProfile.Picture.Split('=').FirstOrDefault();


                var jwt = GenerateToken(user, new[] { Roles.Client });
                return new AuthResponseDto { IsSuccess = true, Data = jwt };
            }

            return new AuthResponseDto { IsSuccess = false, Data = "Unable to login withexternal user" };
        }
         private UserProfileDto ParseGoogleJwtToken(JwtSecurityToken googleToken)
        {
            return new UserProfileDto
            {
                Email = googleToken.Claims.First(c => c.Type == "email").Value,
                VerifiedEmail = bool.Parse(googleToken.Claims.First(c => c.Type == "email_verified").Value),
                Name =googleToken.Claims.First(c => c.Type == "name").Value,
                GivenName = googleToken.Claims.First(c => c.Type == "given_name").Value,
                FamilyName = googleToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
                Locale = googleToken.Claims.First(c => c.Type == "locale").Value,
                Picture = googleToken.Claims.First(c => c.Type == "picture").Value,
            };
        }

        private async Task<CoyoteUser> CreateExternalUserAsync(UserProfileDto userProfile, string externalProvider)
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

        private string GenerateToken(CoyoteUser user, IList<string> userClaims)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>(userClaims.Select(x => new Claim(ClaimTypes.Role, x)))
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName)
            };
            claims.Add(new Claim("Id", user.Id));

            var isAdmin = userClaims.Contains(UserRolesDto.Administrator.ToString());

            var token = new JwtSecurityToken(_jwtSettings.Audience,
                _jwtSettings.Issuer,
                claims,
                expires: isAdmin ? DateTime.Now.AddMinutes(15) : DateTime.Now.AddDays(1),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private async Task<UserProfileDto> GetUserProfileAsync(string token, CancellationToken cancellationToken)
        {
            HttpResponseMessage userInfoResponse;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v1/");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                userInfoResponse = await client.GetAsync("userinfo?alt=json", cancellationToken);
            }

            if (!userInfoResponse.IsSuccessStatusCode)
                return null;

            var userInfoStr = await userInfoResponse.Content.ReadAsStringAsync(cancellationToken);
            var userProfile = JsonConvert.DeserializeObject<UserProfileDto>(userInfoStr);
            return userProfile;
        }
    }
}
