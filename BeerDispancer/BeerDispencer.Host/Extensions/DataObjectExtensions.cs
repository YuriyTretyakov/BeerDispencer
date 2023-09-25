using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Commands;
using BeerDispenser.WebApi.ViewModels.Request;
using BeerDispenser.WebApi.ViewModels.Response;

namespace BeerDispenser.WebApi.Extensions
{
    public static class DataObjectExtensions
	{
        public static Dispencer ToViewModel(this DispenserDto dispencerDto)
        {
            return new Dispencer
            {
                Id = dispencerDto.Id,
                FlowVolume =
                dispencerDto.Volume==null?
                default:
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

