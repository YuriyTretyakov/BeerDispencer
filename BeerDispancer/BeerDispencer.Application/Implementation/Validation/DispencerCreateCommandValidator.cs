using System;
using BeerDispancer.Application.Implementation.Commands;
using FluentValidation;

namespace BeerDispancer.Application.Implementation.Validation
{
	public class DispencerCreateCommandValidator:AbstractValidator<DispencerCreateCommand>
	{
		public DispencerCreateCommandValidator()
		{
			RuleFor(x => x.FlowVolume).NotNull().GreaterThan(0);
		}
	}
}

