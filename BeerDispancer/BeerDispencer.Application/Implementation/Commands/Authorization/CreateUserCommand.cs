using BeerDispenser.Application.Implementation.Response;
using BeerDispenser.Shared;
using BeerDispenser.Shared.Dto;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands.Authorization
{
    public class CreateUserCommand: UserCredentialsDto, IRequest<AuthResponseDto>
    {
	}
}

