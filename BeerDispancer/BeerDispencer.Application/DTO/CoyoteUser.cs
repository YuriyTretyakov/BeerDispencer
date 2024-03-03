using Microsoft.AspNetCore.Identity;

namespace BeerDispenser.Application.DTO
{
    public class CoyoteUser : IdentityUser
    {
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool DisabledByAdmin { get; set; } = false;
        public string ReferenceCode { get; set; }
        public Guid? ReferencedBy { get; set; }
        public string LoginProvider { get; set; } = "Internal";
        public string? PictureUrl { get; set; }

        public CoyoteUser()
        {
            ReferenceCode = GenerateReferenceNumber();
        }

        private string GenerateReferenceNumber()
        {
            string referenceNumber = "";

            var timeStamp = CreatedAt.ToString("YYMMddHH");
            string uniqueIdString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);

            for (int i = 0; i < uniqueIdString.Length - 2; i += 2)
            {
                referenceNumber += uniqueIdString[i] + uniqueIdString[i + 1];
                referenceNumber += timeStamp[i] + timeStamp[i + 1];
            }

            return referenceNumber;
        }
    }
}
