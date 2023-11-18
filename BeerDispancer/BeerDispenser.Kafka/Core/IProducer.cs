namespace BeerDispenser.Kafka.Core
{
    public interface IProducer<T>: IDisposable where T : class
    {
        Task ProduceAsync(string topicName, IReadonlyEventHolder<T> @event, CancellationToken cancellationToken);
    }
}