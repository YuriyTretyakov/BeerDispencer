using BeerDispencer.Domain.Abstractions;
using BeerDispencer.Shared;

namespace BeerDispencer.Domain.Entity
{
    public sealed class Dispencer
    {
        public IReadOnlyCollection<Usage> Usages => _usages;

        public Guid? Id { get; private set; }
        public decimal Volume { get; private set; }
        public DispencerStatus Status { get; private set; }


        private List<Usage> _usages = new();

        private Dispencer(
            Guid? id,
            decimal volume,
            DispencerStatus status)
        {
            Id = id;
            Volume = volume;
            Status = status;
        }


        public Usage Open()
        {
            if (Status == DispencerStatus.Open || Status == DispencerStatus.OutOfService)
            {
                throw new InvalidOperationException($"Invalid dispencer state: {Status}");
            }

            Status = DispencerStatus.Open;

            var usage = Usage.Create(Id.Value);
            _usages.Add(usage);
            return usage;
        }

        public Usage Close(IBeerFlowCalculator calculator)
        {
            if (Status == DispencerStatus.Close || Status == DispencerStatus.OutOfService)
            {
                throw new InvalidOperationException($"Invalid dispencer state: {Status}");
            }

            Status = DispencerStatus.Close;

            var currentUsage = _usages.First(x => x.ClosedAt == null);
            currentUsage.SetClose(calculator);
            return currentUsage;
        }

        internal void SetUsages(IList<Usage> usages)
        {
            _usages = usages.ToList();
        }


        public UsageResponse GetSpendings(IBeerFlowCalculator calculator)
        {
              decimal total = 0;

            var spendings = _usages.Select(x =>
            {
                var entry = new UsageEntry
                {
                    OpenedAt = x.OpenAt,
                    ClosedAt = x.ClosedAt,
                };

                entry.FlowVolume = x.FlowVolume ?? calculator.GetFlowVolume(DateTime.UtcNow, x.OpenAt);
                entry.TotalSpent = x.TotalSpent ?? calculator.GetTotalSpent(entry.FlowVolume);
                total += entry.TotalSpent ?? 0;
                return entry;
            }).ToArray();

            return new UsageResponse { Amount = total, Usages = spendings };
        }


        public static Dispencer CreateNewDispencer(decimal volume)
        {
            return new Dispencer(null, volume, DispencerStatus.Close);
        }


        public static Dispencer Create(
            Guid id,
            decimal volume,
            DispencerStatus status,
            IList<Usage> usages)
        {
            var dispencer = new Dispencer(id, volume, status);
            dispencer.SetUsages(usages);
            return dispencer;
        }
    }
}

