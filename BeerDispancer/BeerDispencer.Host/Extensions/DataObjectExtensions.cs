using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispencer.WebApi.ViewModels.Request;
using BeerDispencer.WebApi.ViewModels.Response;

namespace BeerDispencer.WebApi.Extensions
{
    public static class DataObjectExtensions
	{
        public static Dispencer ToViewModel(this DispencerDto dispencerDto)
        {
            return new Dispencer
            {
                Id = dispencerDto.Id.Value,
                FlowVolume =
                dispencerDto.Volume==null?
                default:
                dispencerDto.Volume.Value };
        }

        public static DispencerDto ToDto(this DispencerCreateCommand dispencerCommand)
        {
            return new DispencerDto { Volume = dispencerCommand.FlowVolume };
        }

        public static DispencerUpdateCommand ToCommand(this DispenserUpdateModel model, Guid id)
        {
            return  new DispencerUpdateCommand { Id = id,
                Status =  model.Status,
                UpdatedAt = model.UpdatedAt };
        }

    }
}

