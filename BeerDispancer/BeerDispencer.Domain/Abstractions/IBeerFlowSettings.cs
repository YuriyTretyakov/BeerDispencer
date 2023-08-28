namespace BeerDispencer.Domain.Abstractions
{
    public interface IBeerFlowSettings
    {
        decimal LitersPerSecond { get; set; }
        decimal PricePerLiter { get; set; }
    }
}