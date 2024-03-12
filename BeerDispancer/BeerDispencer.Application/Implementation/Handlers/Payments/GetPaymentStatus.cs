using System.Net;
using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Queries;
using BeerDispenser.Shared.Dto.Payments;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers.Payments
{
    public class GetPaymentStatus:IRequestHandler<GetPaymentsStatus, HttpStatusCode>
	{
        private readonly IDispencerUof _dispencerUof;

        public GetPaymentStatus(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        public async Task<HttpStatusCode> Handle(GetPaymentsStatus request, CancellationToken cancellationToken)
        {
            var payment = await _dispencerUof.UsageRepo.GetByIdAsync(request.PaymentId);

            return payment?.PaymentStatus == PaymentStatusDto.Success ? HttpStatusCode.OK : HttpStatusCode.NoContent;
        }
    }
}

