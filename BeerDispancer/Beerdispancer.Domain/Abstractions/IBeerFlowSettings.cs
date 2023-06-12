namespace Beerdispancer.Domain.Abstractions
{
    public interface IBeerFlowSettings
    {
        double LitersPerSecond { get; set; }
        double PricePerLiter { get; set; }
    }
}