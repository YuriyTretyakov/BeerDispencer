namespace BeerDispenser.Kafka.Core
{
    public interface IEventPublisher<T> where T : class
    {
        public string ConfigSectionName { get; }

        Task RaiseEventAsync(IReadonlyEventHolder<T> @event, CancellationToken cancellationToken);

        Task RetryAsync(IReadonlyEventHolder<T> @event, CancellationToken cancellationToken);
    }
}