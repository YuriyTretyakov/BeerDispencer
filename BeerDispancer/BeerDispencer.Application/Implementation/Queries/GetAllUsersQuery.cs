using System;
using BeerDispancer.Application.Implementation.Response;
using MediatR;

namespace BeerDispancer.Application.Implementation.Queries
{
	public class GetAllUsersQuery: IRequest<User[]>
	{
	
	}
}

