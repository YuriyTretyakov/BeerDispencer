using System;
using BeerDispencer.Application.Implementation.Response;
using Microsoft.AspNetCore.Identity;

namespace BeerDispencer.Application
{
	public static class Extensions
	{
        public static AuthResponseDto ToAuthResponseDto(this IEnumerable<IdentityError> errors)
        {
            return new AuthResponseDto
            {
                IsSuccess = !errors.Any(),
                ProblemDetails = errors.Select(x => new AuthDetails { Code = x.Code, Description = x.Description })
                    .ToArray()
            };
        }
	}
}

