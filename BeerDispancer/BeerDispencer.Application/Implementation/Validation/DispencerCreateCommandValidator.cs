using System;
using BeerDispenser.Application.Implementation.Commands;
using FluentValidation;

namespace BeerDispenser.Application.Implementation.Validation
{
	public class DispencerCreateCommandValidator:AbstractValidator<DispenserCreateCommand>
	{
		public DispencerCreateCommandValidator()
		{
			RuleFor(x => x.FlowVolume).NotNull().GreaterThan(0);
		}
	}
}

