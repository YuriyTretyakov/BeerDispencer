using BeerDispenser.Application.Implementation.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;

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
           return Task.FromResult(_configuration["OAUTH:Google:ClientId"]);
        }
    }
}
