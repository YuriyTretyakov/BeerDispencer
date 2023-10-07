﻿using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Queries;
using BeerDispenser.Domain.Entity;
using BeerDispenser.Shared;
using MediatR;
using Stripe.Checkout;

namespace BeerDispenser.Application.Implementation.Handlers
{
    public class OrderSuccessHandler : IRequestHandler<GetOrderDetailsQuery, OrderResponseDetails>
    {
        private readonly IDispencerUof _dispencerUof;

        public OrderSuccessHandler(IDispencerUof dispencerUof)
        {
            _dispencerUof = dispencerUof;
        }

        public async Task<OrderResponseDetails> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {

            var sessionService = new SessionService();
            var session = sessionService.Get(request.SessionId);

            if (session is not null)
            {
                var orderDetails = new OrderResponseDetails
                {
                    AmountTotal = session.AmountTotal.Value,
                    Email = session.CustomerDetails.Email,
                    IsSuccess = true
                };

                session.Metadata.TryGetValue(nameof(OrderResponseDetails.ProductId), out var id);
                var dispencerId = Guid.Parse(id);

                using (var transaction = _dispencerUof.StartTransaction())
                {

                    var dispenserDto = await _dispencerUof
                       .DispencerRepo
                       .GetByIdAsync(dispencerId);

                    var dispenser = Dispenser
                        .CreateDispenser(
                        dispenserDto.Id,
                        dispenserDto.Volume.Value,
                        dispenserDto.Status.Value,
                        dispenserDto.IsActive.Value);


                    var usageDto = dispenser
                        .Reserve(orderDetails.Email, orderDetails.AmountTotal)
                        .ToDto();

                    dispenserDto = dispenser.ToDto();

                    await _dispencerUof.UsageRepo.AddAsync(usageDto);
                    await _dispencerUof.DispencerRepo.UpdateAsync(dispenserDto);
                    await _dispencerUof.Complete();
                    _dispencerUof.CommitTransaction();
                }

                return orderDetails;
            }

            return default;

        }
    }
}