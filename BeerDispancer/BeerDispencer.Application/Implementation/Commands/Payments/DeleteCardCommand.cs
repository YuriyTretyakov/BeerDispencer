using System;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands.Payments
{
	public class DeleteCardCommand: IRequest<bool>
    {
		public Guid UserId { get; set; }
        public Guid CardId { get; set; }
    }
}

