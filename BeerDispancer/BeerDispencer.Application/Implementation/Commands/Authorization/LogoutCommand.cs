using System;
using BeerDispenser.Application.Implementation.Response;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands.Authorization
{
	public class LogoutCommand:IRequest<AuthResponseDto>
	{
		
	}
}

