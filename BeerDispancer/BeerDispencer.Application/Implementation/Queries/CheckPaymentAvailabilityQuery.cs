using BeerDispenser.Shared.Dto.Payments;
using MediatR;

namespace BeerDispenser.Application.Implementation.Queries
{
    public class CheckPaymentAvailabilityQuery:IRequest<PaymentAvailabilityDto>
    {
       public Guid UserId { get; set; }
    }
}
