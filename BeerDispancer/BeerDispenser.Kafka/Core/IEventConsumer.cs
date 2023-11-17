namespace BeerDispenser.Kafka.Core
{
    public interface IEventConsumer<T> : IDisposable where T : class
    {
        public string ConfigSectionName { get; }
        EventHolder<T> GetMessages();
        void StartConsuming(CancellationToken cancellationToken);
        void Stop(CancellationToken cancellationToken);
    }
}