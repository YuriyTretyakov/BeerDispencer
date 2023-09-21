using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispencer.WebApi.ViewModels.Request;
using BeerDispencer.WebApi.ViewModels.Response;

namespace BeerDispencer.WebApi.Extensions
{
    public static class DataObjectExtensions
	{
        public static Dispencer ToViewModel(this DispenserDto dispencerDto)
        {
            return new Dispencer
            {
                Id = dispencerDto.Id.Value,
                FlowVolume =
                dispencerDto.Volume==null?
                default(decimal):
                dispencerDto.Volume.Value };
        }

        public static DispenserDto ToDto(this DispenserCreateCommand dispencerCommand)
        {
            return new DispenserDto { Volume = dispencerCommand.FlowVolume };
        }

        public static DispenserUpdateCommand ToCommand(this DispenserUpdateModel model, Guid id)
        {
            return  new DispenserUpdateCommand { Id = id,
                Status =  model.Status,
                UpdatedAt = model.UpdatedAt };
        }

    }
}

