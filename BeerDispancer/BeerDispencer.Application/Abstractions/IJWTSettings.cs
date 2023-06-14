namespace BeerDispencer.Application.Abstractions
{
    public interface IJWTSettings
    {
        string Issuer { get; set; }
        string Audience { get; set; }
        string Secret { get; set; }
    }
}