using System;
using BeerDispenser.Application.DTO;
using MediatR;

namespace BeerDispenser.Application.Implementation.Queries
{
	public class GetActiveDispensersQuery:IRequest<DispenserDto[]>
	{
	
	}
}

