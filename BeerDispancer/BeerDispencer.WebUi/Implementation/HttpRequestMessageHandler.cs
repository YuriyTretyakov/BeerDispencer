using BeerDispenser.WebUi.Abstractions;
using System.Net;

namespace BeerDispenser.WebUi.Implementation
{
    public class HttpRequestMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorage _localStorage;
        private readonly UserNotificationService _notificationService;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetStringAsync("user");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await base.SendAsync(request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                await Task.Run(_notificationService.ShowSuccessNotification);
               
            }
            else
            {
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                   await Task.Run(_notificationService.ShowUnauthorizedNotification);
                   
                }
                else
                {
                   
                    var content = await response.Content.ReadAsStringAsync();
                    await Task.Run(()=> _notificationService.ShowErrorNotification(content));
                }
            }

            return response;
        }

        public HttpRequestMessageHandler(
            ILocalStorage localStorage,
            UserNotificationService notificationService)
        {
            _localStorage = localStorage;
            _notificationService = notificationService;
        }
    }
}

