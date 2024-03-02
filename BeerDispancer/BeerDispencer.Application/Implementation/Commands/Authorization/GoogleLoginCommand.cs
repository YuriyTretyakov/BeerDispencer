using BeerDispenser.Application.Implementation.Response;
using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands.Authorization
{
    public class GoogleLoginCommand:IRequest<AuthResponseDto>
    {
        public string Code { get; set; }
    }
}
