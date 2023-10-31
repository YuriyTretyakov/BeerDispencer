namespace BeerDispenser.Domain.Entity
{
    public class PaymentCard : Entity
    {
        public Guid UserId { get; private set; }
        public string CardId { get; private set; }
        public string CustomerId { get; private set; }
        public bool IsDefault { get; internal set; }
        public string City { get; private set; }
        public string AdressCountry { get; private set; }
        public string Line1 { get; private set; }
        public string State { get; private set; }
        public string Zip { get; private set; }
        public string Brand { get; private set; }
        public string Country { get; private set; }
        public string CvcCheck { get; private set; }
        public string Dynamiclast4 { get; private set; }
        public string ExpMonth { get; private set; }
        public string ExpYear { get; private set; }
        public string Last4 { get; private set; }
        public string AccountHolderName { get; private set; }
        public string ClientIp { get; private set; }
        public int Created { get; private set; }
        public string Email { get; private set; }
        

        public void Validate()
        {

        }

        public override string ToString()
        {
            return $"{Brand}:{Last4}:{ExpMonth}/{ExpYear}";
        }


        public static PaymentCard Create(
            Guid id,
            Guid userId,
            string cardId,
            string tokenId,
            bool isDefault,
            string city,
            string adressCountry,
            string line1,
            string state,
            string zip,
            string brand,
            string country,
            string cvcCheck,
            string dlast4,
            string expMonth,
            string expYear,
            string lasy4,
            string name,
            string clientIp,
            int created,
            string email)
        {
            return new PaymentCard
            {
                Id = id,
                UserId = userId,
                CardId = cardId,
                CustomerId = tokenId,
                IsDefault = isDefault,
                City = city,

                AdressCountry = adressCountry,
                Line1 = line1,
                State = state,
                Zip = zip,
                Brand = brand,
                Country = country,
                CvcCheck = cvcCheck,
                Dynamiclast4 = dlast4,
                ExpMonth = expMonth,
                ExpYear = expYear,
                Last4 = lasy4,
                AccountHolderName = name,
                ClientIp = clientIp,
                Created = created,
                Email = email
            };
        }

        public static PaymentCard Create(

            Guid userId,
            string cardId,
            string customerId,
            bool isDefault,
            string city,
            string adressCountry,
            string line1,
            string state,
            string zip,
            string brand,
            string country,
            string cvcCheck,
            string dlast4,
            string expMonth,
            string expYear,
            string lasy4,
            string name,
            string clientIp,
            int created,
            string email)
        {
            return new PaymentCard
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CardId = cardId,
                CustomerId = customerId,
                IsDefault = isDefault,
                City = city,

                AdressCountry = adressCountry,
                Line1 = line1,
                State = state,
                Zip = zip,
                Brand = brand,
                Country = country,
                CvcCheck = cvcCheck,
                Dynamiclast4 = dlast4,
                ExpMonth = expMonth,
                ExpYear = expYear,
                Last4 = lasy4,
                AccountHolderName = name,
                ClientIp = clientIp,
                Created = created,
                Email = email
            };
        }
    }
}

