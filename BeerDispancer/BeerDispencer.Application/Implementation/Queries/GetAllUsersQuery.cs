using System;
using BeerDispenser.Application.Implementation.Response;
using MediatR;

namespace BeerDispenser.Application.Implementation.Queries
{
	public class GetAllUsersQuery: IRequest<User[]>
	{
	
	}
}

