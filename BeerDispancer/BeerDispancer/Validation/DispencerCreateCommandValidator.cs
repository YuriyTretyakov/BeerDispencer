using System;
using BeerDispencer.WebApi.Commands;
using FluentValidation;

namespace BeerDispencer.WebApi.Validation
{
	public class DispencerCreateCommandValidator:AbstractValidator<DispencerCreateCommand>
	{
		public DispencerCreateCommandValidator()
		{
			RuleFor(x => x.FlowVolume).NotNull().NotEqual(0);
		}
	}
}

