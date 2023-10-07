using System;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands
{
	public class DeactivateDispenserCommand:IRequest<(bool Result, string Details)>
	{
		public Guid Id { get; set; }
	}
}

