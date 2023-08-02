using System;
using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.DTO;
using BeerDispencer.Application.Implementation.Commands;
using BeerDispencer.Application.Implementation.Response;
using MediatR;

namespace BeerDispencer.Application.Implementation.Handlers
{
    public class ProcessPaymentHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
    {
        private readonly IDispencerUOW _dispencerUof;

        public ProcessPaymentHandler(IDispencerUOW dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
          
                var dispencerDto = await _dispencerUof.DispencerRepo.GetByIdAsync(request.DispencerId);


                if (dispencerDto == null || dispencerDto.Status == DispencerStatusDto.Open)
                {
                    return new CreatePaymentResponse { IsSuccess = false, Data = $"Invalid status of dispenceer {nameof(DispencerStatusDto.Open)}" };
                }

               await _dispencerUof.ProcessPaymentAsync(request.DispencerId, request.Amount);
                return new CreatePaymentResponse { IsSuccess = true, Data = $"Payment initiated for dispenceer {request.DispencerId}" };
        }
    }
}

