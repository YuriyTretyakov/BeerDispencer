using System;
using BeerDispenser.Shared.Dto;
using BeerDispenser.Shared.Dto.Payments;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands
{
    public class ClientOperationsCommand : IRequest<PaymentRequiredDto>
	{
	    public Guid UserId { get; set; }
		public Guid Id { get; set; }
		public DispenserStatusDto Status { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
	}
	
}

