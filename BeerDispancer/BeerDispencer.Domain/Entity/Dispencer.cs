using BeerDispencer.Domain.Abstractions;
using BeerDispencer.Shared;

namespace BeerDispencer.Domain.Entity
{
    public sealed class Dispencer: EntityBase
    {
        public decimal Volume { get; private set; }
        public DispencerStatus Status { get; private set; }

        private IBeerFlowSettings _beerFlowSettings;
        public IReadOnlyCollection<Usage> Usages => _usages;

        private List<Usage> _usages = new List<Usage>();

        private Dispencer(
            Guid id,
            decimal volume,
            DispencerStatus status,
            IBeerFlowSettings beerFlowSettings)
        {
            Id = id;
            Volume = volume;
            Status = status;
            _beerFlowSettings = beerFlowSettings;
        }


        public void Open()
        {
            if (Status == DispencerStatus.Open || Status == DispencerStatus.OutOfService)
            {
                throw new InvalidOperationException($"Invalid dispencer state: {Status}");
            }

            Status = DispencerStatus.Open;

            var usage = Usage.Create(Id, _beerFlowSettings);
            _usages.Add(usage);
        }

        public void Close()
        {
            if (Status == DispencerStatus.Close || Status == DispencerStatus.OutOfService)
            {
                throw new InvalidOperationException($"Invalid dispencer state: {Status}");
            }

            Status = DispencerStatus.Close;

            var currentUsage = _usages.First(x => x.ClosedAt == null);
            currentUsage.SetClose();
        }

        internal void SetUsages(IList<Usage> usages)
        {
            _usages = usages.ToList();
        }


        public static Dispencer CreateNewDispencer(decimal volume, IBeerFlowSettings beerFlowSettings)
        {
            return new Dispencer(Guid.NewGuid(), volume, DispencerStatus.Close, beerFlowSettings);
        }


        public static Dispencer CreateDispencer(
            Guid id,
            decimal volume,
            DispencerStatus status,
            IList<Usage> usages,
            IBeerFlowSettings beerFlowSettings)
        {
            var dispencer = new Dispencer(id, volume, status, beerFlowSettings);
            dispencer.SetUsages(usages);
            return dispencer;
        }

    }
}

