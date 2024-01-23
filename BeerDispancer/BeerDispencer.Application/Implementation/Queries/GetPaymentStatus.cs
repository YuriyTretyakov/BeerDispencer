using System.Net;
using BeerDispenser.Application.DTO;
using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Queries
{
    public class GetPaymentsStatus : IRequest<HttpStatusCode>
    {
        public Guid PaymentId { get; set; }
    }
}

