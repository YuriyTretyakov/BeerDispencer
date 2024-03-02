using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace BeerDispenser.WebUi.Implementation.ExternalLogin.Google;

public class GoogleAuthenticationStateProvider : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new(new ClaimsPrincipal());
    }

    [JSInvokable]
    public void GoogleLogin(GoogleResponse googleResponse)
    {
        Console.WriteLine("GoogleLogin " + googleResponse.Credential);
        var principal = new ClaimsPrincipal();
        var user = User.FromGoogleJwt(googleResponse.Credential);
        // CurrentUser = user;

        if (user is not null)
        {
            principal = user.ToClaimsPrincipal();
        }
    }
}
