using System.Collections.ObjectModel;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Response;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using BeerDispenser.Domain.Entity;

namespace BeerDispenser.Application
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
                Status = domainDispenser.Status,
                ReservedFor = domainDispenser.ReservedFor
            };
        }

        public static ReadOnlyCollection<Usage> ToDomain(this IEnumerable<UsageDto> dto,
            IBeerFlowSettings beerFlowSettings)
        {

            return dto.Select(x =>

                 Usage.Create(

                 x.Id,
                  x.DispencerId,
                  x.OpenAt,
                  beerFlowSettings)).ToList().AsReadOnly();

        }

        public static IEnumerable<Usage> ToDomain(this IEnumerable<UsageDto> dto)
        {

            return dto.Select(x =>

                 Usage.Create(

                 x.Id,
                  x.DispencerId,
                  x.OpenAt,
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

