using System;
using System.Net;
using BeerDispenser.WebUi.Abstractions;
using Microsoft.AspNetCore.Components;

namespace BeerDispenser.WebUi.Implementation
{
    public class HttpRequestMessageHandler : DelegatingHandler
    {
        private readonly UserNotificationService _notificationService;
        private readonly ILocalStorage _localStorage;
        private readonly NavigationManager _navManager;


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetStringAsync("user");

            if (token is not null)
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
            }

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
                    await _localStorage.RemoveAsync("user");
                    response.StatusCode = HttpStatusCode.OK;
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
            UserNotificationService notificationService,
            ILocalStorage localStorage, NavigationManager navManager
            )
        {
           
            _notificationService = notificationService;
            _localStorage = localStorage;
            _navManager = navManager;
        }
    }
}

