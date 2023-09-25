using BeerDispenser.Application.Implementation.Response;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands.Authorization
{
    public class CreateUserCommand:UserLoginCommand, IRequest<AuthResponseDto>
    {
		public string Role { get; set; }
	}
}

