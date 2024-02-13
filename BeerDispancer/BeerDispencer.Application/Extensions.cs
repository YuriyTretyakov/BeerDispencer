using System.Collections.ObjectModel;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Response;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using BeerDispenser.Shared;
using Microsoft.AspNetCore.Identity;


namespace BeerDispenser.Application
{
    public static class Extensions
    {
        public static AuthResponseDto ToAuthResponseDto(this IEnumerable<IdentityError> errors)
        {
            return new AuthResponseDto
            {
                IsSuccess = !errors.Any(),
                ProblemDetails = errors.Select(x => new AuthDetails { Code = x.Code, Description = x.Description })
                    .ToArray()
            };
        }

        public static DispenserDto ToDto(this Dispenser domainDispenser)
        {
            return new DispenserDto
            {
                Id = domainDispenser.Id,
                Volume = domainDispenser.Volume,
                Status = domainDispenser.Status,
                IsActive = domainDispenser.IsActive
            };
        }

        public static ReadOnlyCollection<Usage> ToDomain(this IEnumerable<UsageDto> dto,
            IBeerFlowSettings beerFlowSettings)
        {

            return dto.Select(x =>

                 Usage.Create(

                 x.Id,
                  x.DispencerId,
                  x.OpenAt,
                  x.ClosedAt,
                  x.FlowVolume,
                  x.TotalSpent,
                  beerFlowSettings)).ToList().AsReadOnly();
        }

        public static UsageDto ToDto(this Usage usage)
        {

            return new UsageDto
            {
                Id = usage.Id,
                DispencerId = usage.DispencerId,
                OpenAt = usage.OpenAt,
                ClosedAt = usage.ClosedAt,
                TotalSpent = usage.TotalSpent,
                FlowVolume = usage.FlowVolume
            };
        }

        public static PaymentCard ToDomain(this PaymentCardDto paymentCard)
        {

            return PaymentCard.Create(
             paymentCard.Id,
                paymentCard.UserId,
                paymentCard.CardId,
                paymentCard.CustomerId,
                paymentCard.IsDefault,
                paymentCard.City,
                paymentCard.AdressCountry,
               paymentCard.Line1,
                paymentCard.State,
                paymentCard.Zip,
                paymentCard.Brand,
                paymentCard.Country,
                paymentCard.CvcCheck,
                paymentCard.Dynamiclast4,
                paymentCard.ExpMonth,
               paymentCard.ExpYear,
                paymentCard.Last4,
                 paymentCard.AccountHolderName,
                 paymentCard.ClientIp,
                paymentCard.Created,
                 paymentCard.Email);
        }

        public static PaymentCardDto ToDto(this PaymentCard paymentCard)
        {

            return new PaymentCardDto
            {
                Id = paymentCard.Id,
                UserId = paymentCard.UserId,
                CardId = paymentCard.CardId,
                CustomerId = paymentCard.CustomerId,
                IsDefault = paymentCard.IsDefault,
                City = paymentCard.City,
                AdressCountry = paymentCard.AdressCountry,
                Line1 = paymentCard.Line1,
                State = paymentCard.State,
                Zip = paymentCard.Zip,
                Brand = paymentCard.Brand,
                Country = paymentCard.Country,
                CvcCheck = paymentCard.CvcCheck,
                Dynamiclast4 = paymentCard.Dynamiclast4,
                ExpMonth = paymentCard.ExpMonth,
                ExpYear = paymentCard.ExpYear,
                Last4 = paymentCard.Last4,
                AccountHolderName = paymentCard.AccountHolderName,
                ClientIp = paymentCard.ClientIp,
                Created = paymentCard.Created,
                Email = paymentCard.Email
            };
        }

        public static PaymentCard ToDomain(this StripeTokenResponse tokenResponse, string customerId, Guid userId)
        {
            return PaymentCard.Create(
                userId,
                tokenResponse.Token.Card.Id,
                customerId,
                false,
                tokenResponse.Token.Card.City,
                tokenResponse.Token.Card.AdressCountry,
                tokenResponse.Token.Card.Line1,
                tokenResponse.Token.Card.State,
                tokenResponse.Token.Card.Zip,
                tokenResponse.Token.Card.Brand,
                tokenResponse.Token.Card.Country,
                tokenResponse.Token.Card.CvcCheck,
                tokenResponse.Token.Card.Dynamiclast4,
                tokenResponse.Token.Card.ExpMonth,
                tokenResponse.Token.Card.ExpYear,
                tokenResponse.Token.Card.Last4,
                tokenResponse.Token.Card.Name,
                tokenResponse.Token.ClientIp,
                tokenResponse.Token.Created,
                tokenResponse.Token.Email
            );
        }

    }
}

