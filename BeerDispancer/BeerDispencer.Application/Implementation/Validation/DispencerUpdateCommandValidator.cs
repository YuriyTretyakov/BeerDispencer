﻿using System;
using BeerDispancer.Application.Implementation.Commands;
using FluentValidation;

namespace BeerDispancer.Application.Implementation.Validation
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

