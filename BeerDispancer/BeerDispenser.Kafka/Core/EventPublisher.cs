using Microsoft.Extensions.Logging;

namespace BeerDispenser.Messaging.Core
{
    public abstract class EventPublisher<T> : IDisposable where T : class
    {
        private readonly Producer<T> _producer;

        public EventPublisher(EventHubConfig configuration, ILogger logger)
        {
            _producer = new Producer<T>(configuration, logger);
        }

        public async Task RaiseEventAsync(IReadonlyEventHolder<T> @event, CancellationToken cancellationToken)
        {
            await _producer
                .ProduceAsync(@event, cancellationToken);
        }

        public async Task RetryAsync(IReadonlyEventHolder<T> @event, CancellationToken cancellationToken)
        {
            @event.IncrementRetries();
            await _producer
                .ProduceAsync(@event, cancellationToken);
        }

        public void Dispose()
        {
            //_producer.DisposeAsync();
        }
    }
}

