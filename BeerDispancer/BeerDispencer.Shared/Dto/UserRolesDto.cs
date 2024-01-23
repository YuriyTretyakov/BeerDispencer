namespace BeerDispenser.Shared.Dto
{
    public enum UserRolesDto
    {
        Unknown,
        Operator,
        Administrator,
        Client
    }

    public static class Roles
    {
        public const string Unknown = "Unknown";
        public const string Operator = "Operator";
        public const string Administrator = "Administrator";
        public const string Client = "Client";
    }
}

