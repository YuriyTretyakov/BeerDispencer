using System;
using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Commands;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers
{
	public class DeactivateDispenserHandler:IRequestHandler<DeactivateDispenserCommand, (bool Result, string Details)>
	{
        private readonly IDispencerUof _dispencerUof;

        public DeactivateDispenserHandler(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        public async Task<(bool Result, string Details)> Handle(DeactivateDispenserCommand request, CancellationToken cancellationToken)
        {
            var dispenserDto = await _dispencerUof.DispencerRepo.GetByIdAsync(request.Id);

            if (dispenserDto is null)
            {
                return await Task.FromResult((false, "$Dispenser with id {request.Id} not found"));
            }

            using (_dispencerUof.StartTransaction())
            {
                dispenserDto.IsActive = false;
                await _dispencerUof.DispencerRepo.UpdateAsync(dispenserDto);
                await _dispencerUof.Complete();
                _dispencerUof.CommitTransaction();
            }
           

            return await Task.FromResult((true, string.Empty));
        }
    }
}

