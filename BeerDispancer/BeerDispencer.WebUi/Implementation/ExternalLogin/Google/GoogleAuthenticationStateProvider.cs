using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BeerDispenser.WebUi.Implementation.ExternalLogin.Google;

public class GoogleAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AccountService _accountService;

    public GoogleAuthenticationStateProvider(AccountService accountService)
    {
        _accountService = accountService;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new(new ClaimsPrincipal());
    }

    [JSInvokable]
    public async Task GoogleLoginAsync(GoogleResponse googleResponse)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.CanReadToken(googleResponse.Credential))
        {
            Console.WriteLine("GoogleLogin " + googleResponse.Credential);

            await _accountService.ProcessExternalUserAsync(googleResponse.Credential);
             
        }
    }
}
