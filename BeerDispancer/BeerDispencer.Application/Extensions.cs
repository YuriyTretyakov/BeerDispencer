using System;
using System.Collections.ObjectModel;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Implementation.Response;
using BeerDispencer.Domain.Abstractions;
using BeerDispencer.Domain.Entity;
using BeerDispenser.Domain.Entity;
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

        public static DispenserDto ToDto(this Dispenser domainDispenser)
        {
            return new DispenserDto
            {
                Id = domainDispenser.Id,
                Volume = domainDispenser.Volume,
                Status = domainDispenser.Status
            };
        }

        public static ReadOnlyCollection<Usage> ToDomain(this IEnumerable<UsageDto> dto, IBeerFlowSettings beerFlowSettings)
        {

            return dto.Select(x =>
             
                 Usage.Create(

                 x.Id,
                  x.DispencerId,
                  x.OpenAt,
                  beerFlowSettings,
                  x.ClosedAt,
                  x.TotalSpent,
                  x.FlowVolume)).ToList().AsReadOnly();
        }

        public static UsageDto ToDto(this Usage usage)
        {

            return new UsageDto
            {
                Id = usage.Id,
                DispencerId = usage.DispencerId,
                OpenAt = usage.OpenAt,
                ClosedAt = usage.ClosedAt,
                TotalSpent = usage.TotalSpent,
                FlowVolume = usage.FlowVolume
            };
        }
    }
}

