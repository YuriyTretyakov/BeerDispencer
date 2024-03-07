using BeerDispenser.Shared.Dto.ExternalProviders;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;

namespace BeerDispenser.WebUi.Implementation.ExternalLogin.Google;

public class ExternalLoginCallbackHandler
{
    private readonly AccountService _accountService;

    public ExternalLoginCallbackHandler(AccountService accountService)
    {
        _accountService = accountService;
    }

    [JSInvokable]
    public async Task GoogleLoginAsync(GoogleResponse googleResponse)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.CanReadToken(googleResponse.Credential))
        {
            await _accountService.ProcessExternalUserAsync(googleResponse.Credential);
        }
    }

    [JSInvokable]
    public async Task FacebookLoginAsync(FaceBookResponse facebookResponse)
    { 
        await _accountService.ProcessExternalFaceBookUserAsync(facebookResponse);
    }
}
