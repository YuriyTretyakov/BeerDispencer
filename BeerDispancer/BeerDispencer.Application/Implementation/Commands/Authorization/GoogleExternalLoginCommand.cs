using BeerDispenser.Application.Implementation.Response;
using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands.Authorization
{
    public class GoogleExternalLoginCommand:IRequest<AuthResponseDto>
    {
        public string GoogleJwt { get; set; }
    }
}
