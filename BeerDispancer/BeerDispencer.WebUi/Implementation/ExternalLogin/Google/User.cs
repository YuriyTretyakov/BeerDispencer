
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BeerDispenser.WebUi.Implementation.ExternalLogin.Google;

public class User
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";


    public static User? FromGoogleJwt(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
       
        if (tokenHandler.CanReadToken(token))
        {
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

            return new()
            {
                Username = jwtSecurityToken.Claims.First(c => c.Type == "name").Value,
                Password = ""
            };
        }

        return null;
    }
}