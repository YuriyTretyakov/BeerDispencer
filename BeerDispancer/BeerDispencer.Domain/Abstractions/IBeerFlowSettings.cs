namespace BeerDispenser.Domain.Abstractions
{
    public interface IBeerFlowSettings
    {
        decimal LitersPerSecond { get; set; }
        decimal PricePerLiter { get; set; }
    }
}