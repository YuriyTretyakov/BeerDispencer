namespace BeerDispancer.Application.Abstractions
{
    public interface IBeerFlowSettings
    {
        double LitersPerSecond { get; set; }
        double PricePerLiter { get; set; }
    }
}