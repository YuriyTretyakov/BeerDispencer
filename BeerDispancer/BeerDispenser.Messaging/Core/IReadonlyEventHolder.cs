namespace BeerDispenser.Messaging.Core
{
    public interface IReadonlyEventHolder<T> where T : class
    {
        Guid Key { get; }
        T Event { get; }
        string Type { get; }
        DateTimeOffset CreatedAt { get; }
        Guid CorrelationId { get; }
        int RetryCount { get; }
        void IncrementRetries();
    }
}