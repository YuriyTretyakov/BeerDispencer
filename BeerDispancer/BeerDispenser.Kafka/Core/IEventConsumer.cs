namespace BeerDispenser.Kafka.Core
{
    public interface IEventConsumer<T> : IDisposable where T : class
    {
        public string ConfigSectionName { get; }
        IReadonlyEventHolder<T> GetMessages();
        void StartConsuming(CancellationToken cancellationToken);
        void Stop(CancellationToken cancellationToken);
    }
}