namespace BeerDispenser.Kafka.Core
{
    public interface IEventConsumer<T> : IDisposable where T : class
    {
        public string ConfigSectionName { get; }
        void StartConsuming(CancellationToken cancellationToken);
        void Stop(CancellationToken cancellationToken);
        Task<EventHolder<T>> ConsumeAsync(CancellationToken cancellationToken);
    }
}