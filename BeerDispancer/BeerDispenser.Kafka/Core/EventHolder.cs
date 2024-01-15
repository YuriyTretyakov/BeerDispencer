namespace BeerDispenser.Kafka.Core
{
    public class EventHolder<T> : IReadonlyEventHolder<T> where T : class
    {

        public string Type => typeof(T).Name;
        public DateTimeOffset CreatedAt { get; set; }
        public Guid Key { get;  set; } 
        public T Event { get;  set; }
        public int Offset { get; set; }
        public Guid CorrelationId { get;  set; }

        public int RetryCount { get;  set; }

        public EventHolder()
        { 
        }

        public EventHolder(T @event)
        {
            Key = Guid.NewGuid();
            Event = @event;
            CreatedAt = DateTimeOffset.Now;
            CorrelationId = Guid.NewGuid();
        }

        protected EventHolder(T @event, int retries, Guid correlationId)
        {
            Key = Guid.NewGuid();
            CreatedAt = DateTimeOffset.Now;
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

