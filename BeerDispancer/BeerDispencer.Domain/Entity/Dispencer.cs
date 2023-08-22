using BeerDispencer.Domain.Abstractions;
using BeerDispencer.Shared;

namespace BeerDispencer.Domain.Entity
{
    public class Dispencer
    {
        public IReadOnlyCollection<Usage> Usages => _usages;

        public Guid Id { get; private set; }
        public double Volume { get; private set; }
        public DispencerStatus Status { get; private set; }


        private List<Usage> _usages = new List<Usage>();

        private Dispencer(
            Guid id,
            double volume,
            DispencerStatus status)
        {
            Id = id;
            Volume = volume;
            Status = status;
        }


        public void Open()
        {
            if (Status == DispencerStatus.Open | Status == DispencerStatus.OutOfService)
            {
                throw new InvalidOperationException($"Invalid dispencer state: {Status}");
            }

            Status = DispencerStatus.Open;

            var usage = Usage.CreateNew(Id);
            _usages.Add(usage);
        }

        public void Close(IBeerFlowCalculator calculator)
        {
            if (Status == DispencerStatus.Close | Status == DispencerStatus.OutOfService)
            {
                throw new InvalidOperationException($"Invalid dispencer state: {Status}");
            }

            Status = DispencerStatus.Close;

            var currentUsage = _usages.First(x => x.ClosedAt == null);
            currentUsage.SetClose(calculator);
        }

        internal void SetUsages(IList<Usage> usages)
        {
            _usages = usages.ToList();
        }


        public static Dispencer CreateNewDispencer(double volume)
        {
            return new Dispencer(Guid.NewGuid(), volume, DispencerStatus.Close);
        }


        public static Dispencer CreateDispencer(
            Guid id,
            double volume,
            DispencerStatus status,
            IList<Usage> usages)
        {
            var dispencer = new Dispencer(id, volume, status);
            dispencer.SetUsages(usages);
            return dispencer;
        }

    }
}

