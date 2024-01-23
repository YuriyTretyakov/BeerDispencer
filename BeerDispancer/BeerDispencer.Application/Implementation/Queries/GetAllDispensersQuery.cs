using System;
using BeerDispenser.Application.DTO;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands
{
	public class GetAllDispensersQuery : IRequest<DispenserDto[]>
    {
	}
}

