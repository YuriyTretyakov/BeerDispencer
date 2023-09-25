using System;
using BeerDispenser.Application.Implementation.Commands;
using FluentValidation;

namespace BeerDispenser.Application.Implementation.Validation
{
	public class DispenserUpdateCommandValidator:AbstractValidator<DispenserUpdateCommand>
	{
		public DispenserUpdateCommandValidator()
		{
			RuleFor(x => x.Id).NotNull().NotEmpty();
			RuleFor(x=>x.Status).NotNull();//.IsInEnum<> is a enum which in Domain which cannot be referenced by Presentation level
			RuleFor(x => x.UpdatedAt).NotNull().NotEmpty().Must(x => !x.Equals(default(DateTime)));
		}
	}
}

