using System;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands.Payments
{
	public class SetDefaultCardCommand:IRequest<bool>
    {
		public Guid CardId { get; set; }
		public Guid UserId { get; set; }
	}
}

