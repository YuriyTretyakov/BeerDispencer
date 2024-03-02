using BeerDispenser.Application.Implementation.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
    internal class GoogleConsentUrlHandler : IRequestHandler<GoogleConsentUrlQuery, string>
    {
        private readonly IConfiguration _configuration;

        public GoogleConsentUrlHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> Handle(GoogleConsentUrlQuery request, CancellationToken cancellationToken)
        {
            var clientId = _configuration["OAUTH:Google:ClientId"];
            var callBackUrl = _configuration["OAUTH:Google:CallBackUrl"]; ;
            return Task.FromResult($"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={callBackUrl}&scope=https://www.googleapis.com/auth/userinfo.email&response_type=code");
        }
    }
}
