using System;
using BeerDispencer.Application.Implementation.Response;
using MediatR;

namespace BeerDispencer.Application.Implementation.Commands.Authorization
{
	public class LogoutCommand:IRequest<AuthResponseDto>
	{
		
	}
}

