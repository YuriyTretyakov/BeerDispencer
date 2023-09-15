using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Implementation.Response;
using BeerDispencer.Domain.Entity;
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

        public static DispencerDto ToDto(this Dispencer domainDispencer)
        {
            return new DispencerDto
            {
                Id = domainDispencer.Id,
                Volume = domainDispencer.Volume,
                Status = domainDispencer.Status,
                ReservedFor = domainDispencer.ReservedFor
            };
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
                  x.FlowVolume));
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

