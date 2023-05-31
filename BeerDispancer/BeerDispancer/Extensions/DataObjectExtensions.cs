
using System;
using Beerdispancer.Domain.Entities;
using Beerdispancer.Infrastructure.DTO;
using BeerDispencer.WebApi.Commands;

namespace BeerDispencer.WebApi.Extensions
{
	public static class DataObjectExtensions
	{
        public static DispencerResponse ToViewModel(this DispencerDto dispencerDto)
        {
            return new DispencerResponse { Id = dispencerDto.Id, FlowVolume = dispencerDto.Volume };
        }

        public static DispencerDto ToDto(this DispencerCreateCommand dispencerCommand)
        {
            return new DispencerDto { Volume = dispencerCommand.FlowVolume };
        }

        public static DispencerUpdateDto ToDto(this DispenserUpdateCommand udpateCommand, Guid id)
        {
            return new DispencerUpdateDto
            { Id = id, Status = udpateCommand.Status, UpdatedAt = udpateCommand.UpdatedAt };
        }

        public static UsageResponse ToViewModel(this SpendingsDto dto)
        {
            var usages = dto.Usages.Select(x =>
             {
                 return new UsageEntry
                 {
                     OpenedAt = x.OpenAt,
                     ClosedAt = x.ClosedAt,
                     FlowVolume = x.FlowVolume,
                     TotalSpent = x.TotalSpent
                 };
             });

            return new UsageResponse { Amount = dto.Amount, Usages = usages.ToArray() };

        }

    }
}

