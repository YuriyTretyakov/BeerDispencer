using System;
using BeerDispencer.Application.Implementation.Response;
using MediatR;

namespace BeerDispancer.Application.Implementation.Commands.Authorization
{
	public class CreateUserCommand:UserLoginCommand, IRequest<AuthResponseDto>
    {
		public string Role { get; set; }
	}
}

