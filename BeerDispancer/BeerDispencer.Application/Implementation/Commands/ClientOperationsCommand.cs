using System;
using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands
{
	public class ClientOperationsCommand : IRequest
	{
	    public Guid UserId { get; set; }
		public Guid Id { get; set; }
		public DispenserStatus Status { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
	}
	
}

