using BeerDispenser.Application.DTO.Authorization;
using BeerDispenser.Shared.Dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BeerDispenser.Application.Implementation
{
    internal class JWtTokenProvider
    {
        private readonly JWTSettings _jwtSettings;

        public JWtTokenProvider(JWTSettings jWTSettings)
        {
            _jwtSettings = jWTSettings;
        }

        public string GenerateToken(CoyoteUser user, IList<string> userClaims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>(userClaims.Select(x => new Claim(ClaimTypes.Role, x)))
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim("Id", user.Id),
                
            };

            if (user.PictureUrl is not null)
            {
                claims.Add(new Claim("picture", user.PictureUrl));
            }

            var isAdmin = userClaims.Contains(UserRolesDto.Administrator.ToString());

            var token = new JwtSecurityToken(_jwtSettings.Audience,
                _jwtSettings.Issuer,
                claims,
                expires: isAdmin ? DateTime.Now.AddMinutes(15) : DateTime.Now.AddDays(1),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
