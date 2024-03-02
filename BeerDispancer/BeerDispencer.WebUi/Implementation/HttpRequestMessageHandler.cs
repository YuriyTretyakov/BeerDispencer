using System.Net;
using System.Net.Http.Json;
using BeerDispenser.Shared;
using BeerDispenser.Shared.Dto;
using BeerDispenser.WebUi.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace BeerDispenser.WebUi.Implementation
{
    public class HttpRequestMessageHandler : DelegatingHandler
    {
        private readonly UserNotificationService _notificationService;
        private readonly ILocalStorage _localStorage;
        private readonly NavigationManager _navManager;
        private readonly IServiceScopeFactory _factory;
        IJSRuntime _jsRuntime;

        public HttpRequestMessageHandler(
           UserNotificationService notificationService,
           ILocalStorage localStorage,
           NavigationManager navManager,
           IServiceScopeFactory factory,
           IJSRuntime jsRuntime)
        {

            _notificationService = notificationService;
            _localStorage = localStorage;
            _navManager = navManager;
            _factory = factory;
            _jsRuntime = jsRuntime;
        }

        public async Task LogAsync(string message)
        {
            await File.AppendAllTextAsync("c:\\work\\webasm.log", DateTimeOffset.UtcNow.ToString("G") + " " + message);
            await _jsRuntime.InvokeVoidAsync("console.log", message);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetStringAsync("user");

            if (token is not null)
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
            }

            var response = await base.SendAsync(request, cancellationToken);


            if (response.StatusCode.Equals(HttpStatusCode.Gone))
            {
                await LogAsync($"status code Gone received");

                var tokenResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                await LogAsync($"Redirection url: {response.StatusCode}");


                await LogAsync($"Tokenvalue: {tokenResponse.Data}");

                //response.
                _navManager.NavigateTo($"/external-login/{tokenResponse.Data}",true);
                //}
                //else
                //{
                //    Console.WriteLine($"ERROR: Unable to redirect");
                //}
            }
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.PaymentRequired)
                {
                    Console.WriteLine($"{HttpStatusCode.PaymentRequired} received redirecting");

                    var contentStr = await response.Content.ReadAsStringAsync();

                    var payment = JsonConvert.DeserializeObject<PaymentRequiredDto>(contentStr);

                    _navManager.NavigateTo($"/paymentInProgress/{payment.PaymentId}", false);
                }

                else if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                    using var scope = _factory.CreateScope();

                    var accountServcice = scope.ServiceProvider.GetRequiredService<AccountService>();

                    await accountServcice.LogoutAsync();
                    _navManager.NavigateTo("/login/");

                    _notificationService.ShowUnauthorizedNotification();
                }
                
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    await Task.Run(() => _notificationService.ShowErrorNotification(content));
                }
            }

            return response;
        }

       
    }
}

