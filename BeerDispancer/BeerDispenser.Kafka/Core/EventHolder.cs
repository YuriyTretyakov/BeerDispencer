namespace BeerDispenser.Kafka.Core
{
    public class EventHolder<T> : IReadonlyEventHolder<T> where T : class
    {

        public string Type => typeof(T).Name;
        public DateTimeOffset CreatedAt { get; set; }
        public Guid Key => Guid.NewGuid();
        public T Event { get; }

        public Guid CorrelationId { get; private set; }

        public int RetryCount { get; private set; }

        public EventHolder(T @event)
        {
            Event = @event;
            CreatedAt = DateTimeOffset.Now;
            CorrelationId = Guid.NewGuid();
        }

        protected EventHolder(T @event, int retries, Guid correlationId)
        {
            Event = @event;
            RetryCount = retries;
            CorrelationId = correlationId;
        }

        public void IncrementRetries()
        {
            RetryCount++;
        }
    }
}

