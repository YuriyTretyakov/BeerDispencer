using BeerDispenser.Shared;
using BeerDispenser.Shared.Dto.ExternalProviders;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands.Authorization
{
    public class FBExternalLoginCommand : IRequest<AuthResponseDto>
    {
        public FaceBookResponse FbResponse { get; set; }
    }
}
