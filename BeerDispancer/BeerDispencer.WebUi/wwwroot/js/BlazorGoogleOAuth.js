let blazorAuthenticationStateProviderInstance = null;

function blazorGoogleInitialize(clientId, blazorAuthenticationStateProvider)
{
    // disable Exponential cool-down
    /*document.cookie = `g_state=;path=/;expires=Thu, 01 Jan 1970 00:00:01 GMT`;*/
    blazorAuthenticationStateProviderInstance = blazorAuthenticationStateProvider;
    google.accounts.id.initialize({ client_id: clientId, callback: blazorCallback });
}

function blazorGooglePrompt()
{
    google.accounts.id.prompt((notification) =>
    {
        if (notification.isNotDisplayed() || notification.isSkippedMoment())
        {
            console.info(notification.getNotDisplayedReason());
            console.info(notification.getSkippedReason());
        }
    });
}

function blazorCallback(googleResponse)
{
    blazorAuthenticationStateProviderInstance.invokeMethodAsync("GoogleLoginAsync", { ClientId: googleResponse.clientId, SelectedBy: googleResponse.select_by, Credential: googleResponse.credential });   
}