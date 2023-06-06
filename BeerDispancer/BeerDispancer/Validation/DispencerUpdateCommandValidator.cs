using System;
using BeerDispencer.WebApi.Commands;
using FluentValidation;

namespace BeerDispencer.WebApi.Validation
{
	public class DispencerUpdateCommandValidator:AbstractValidator<DispencerUpdateCommand>
	{
		public DispencerUpdateCommandValidator()
		{
			RuleFor(x => x.Id).NotNull().NotEmpty();
			RuleFor(x=>x.Status).NotNull();//.IsInEnum<> is a enum which in Domain which cannot be referenced by Presentation level
			RuleFor(x => x.UpdatedAt).NotNull().NotEmpty().Must(x => !x.Equals(default(DateTime)));
		}
	}
}

