using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Application.Implementation.Response;
using BeerDispenser.Shared.Dto.ExternalProviders.Google;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;
using BeerDispenser.Shared.Dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using BeerDispenser.Shared;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
    internal class GoogleSigninCallbackHandler : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
    {
        private readonly string _clientId;
        private readonly string _callBackUrl;
        private readonly string _key;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JWTSettings _jwtSettings;

        public GoogleSigninCallbackHandler(
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            IOptions<JWTSettings> jwtSettings)
        {
            _clientId = configuration["OAUTH:Google:ClientId"];
            _callBackUrl = configuration["OAUTH:Google:CallBackUrl"];
            _key = configuration["OAUTH:Google:Key"];
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var tokenRequest = new TokenRequestDto
            {
                ClientId = _clientId,
                ClientSecret = _key,
                Code = request.Code,
                RedirectUri = _callBackUrl,
                GrantType = "authorization_code"
            };

            HttpResponseMessage tokenResponseMessage;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://oauth2.googleapis.com");
                tokenResponseMessage = await client.PostAsJsonAsync("token", tokenRequest);
            }

            if (!tokenResponseMessage.IsSuccessStatusCode)
                return null;

            var tokenRespString = await tokenResponseMessage.Content.ReadAsStringAsync();
            var googleToken =  JsonConvert.DeserializeObject<TokenResponseDto>(tokenRespString);
            var userProfile = await GetUserProfileAsync(googleToken.AccessToken, cancellationToken);

            var user = await _userManager.FindByEmailAsync(userProfile.Email);

            if (user is  null)
            {
                user =  await CreateExternalUserAsync(userProfile, "Google");
            }

            var jwt = GenerateToken(user, new[] { Roles.Client });
            return new AuthResponseDto { IsSuccess = true, Data = jwt };
        }

        private async Task<IdentityUser> CreateExternalUserAsync(UserProfileDto userProfile, string externalProvider)
        {
            var user = new IdentityUser
            {
                UserName = userProfile?.Name?? userProfile.Email,
                Email = userProfile.Email,
                EmailConfirmed = true,
                NormalizedEmail = userProfile.Email.ToUpper(),
                NormalizedUserName = (userProfile?.Name ?? userProfile.Email).ToUpper(),
            };
            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, Roles.Client);
            return user;
        }

        private string GenerateToken(IdentityUser user, IList<string> userClaims)
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
