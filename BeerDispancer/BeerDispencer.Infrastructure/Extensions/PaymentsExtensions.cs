using System;
using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispenser.Application.DTO;

namespace BeerDispencer.Infrastructure.Extensions
{
    public static class PaymentsExtensions
	{

        public static PaymentCard ToDbEntity(this PaymentCardDto paymentCard)
        {

            return new PaymentCard
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

        public static PaymentCardDto ToDto(this PaymentCard dbCard)
        {

            return new PaymentCardDto
            {
                Id = dbCard.Id,
                UserId = dbCard.UserId,
                CardId = dbCard.CardId,
                CustomerId = dbCard.CustomerId,
                IsDefault = dbCard.IsDefault,
                City = dbCard.City,
                AdressCountry = dbCard.AdressCountry,
                Line1 = dbCard.Line1,
                State = dbCard.State,
                Zip = dbCard.Zip,
                Brand = dbCard.Brand,
                Country = dbCard.Country,
                CvcCheck = dbCard.CvcCheck,
                Dynamiclast4 = dbCard.Dynamiclast4,
                ExpMonth = dbCard.ExpMonth,
                ExpYear = dbCard.ExpYear,
                Last4 = dbCard.Last4,
                AccountHolderName = dbCard.AccountHolderName,
                ClientIp = dbCard.ClientIp,
                Created = dbCard.Created,
                Email = dbCard.Email
            };
        }
    }
}

