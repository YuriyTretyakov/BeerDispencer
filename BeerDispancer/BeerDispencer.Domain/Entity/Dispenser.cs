using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Implementations;
using BeerDispenser.Shared;

namespace BeerDispenser.Domain.Entity
{
    public sealed class Dispenser : Entity
    {
        public decimal Volume { get; private set; }
        public DispenserStatus Status { get; private set; }
        public string ReservedFor { get; private set; }
        public bool IsActive { get; private set; }

        private IBeerFlowSettings _beerFlowSettings;
        public IReadOnlyCollection<Usage> Usages => _usages;


        private List<Usage> _usages = new List<Usage>();

        private Dispenser(
            Guid id,
            decimal volume,
            DispenserStatus status,
            bool isActive,
            IBeerFlowSettings beerFlowSettings = null,
            string reservedFor = null)
        {
            Id = id;
            Volume = volume;
            Status = status;
            _beerFlowSettings = beerFlowSettings;
            ReservedFor = reservedFor;
            IsActive = isActive;
        }


        public Usage Open()
        {
            if (Status == DispenserStatus.Opened || Status == DispenserStatus.OutOfService)
            {
                throw new InvalidOperationException($"Invalid Dispenser state: {Status}");
            }

            if (!IsActive)
            {
                throw new InvalidOperationException($"Unable to operate with deactivated dispenser");
            }

            Status = DispenserStatus.Opened;

            var usage = Usage.CreateStarted(Id, _beerFlowSettings);
            _usages.Add(usage);
            return usage;
        }

        public Usage Close()
        {
            if (Status == DispenserStatus.Closed || Status == DispenserStatus.OutOfService)
            {
                throw new InvalidOperationException($"Invalid Dispenser state: {Status}");
            }

            if (!IsActive)
            {
                throw new InvalidOperationException($"Unable to operate with deactivated dispenser");
            }

            Status = DispenserStatus.Closed;

            var currentUsage = _usages.First(x => x.ClosedAt == null);
            currentUsage.SetClose();
            return currentUsage;
        }

        internal void SetUsages(IList<Usage> usages)
        {
            if (usages != null)
            {
                _usages = usages.ToList();
            }
        }

        public UsageResponse GetSpendings()
        {
            decimal total = 0;
            var calculator = new Calculator(_beerFlowSettings);

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

        public Usage Reserve(string reservedFor, decimal amount)
        {
            if (Status == DispenserStatus.Opened || Status == DispenserStatus.OutOfService)
            {
                throw new InvalidOperationException($"Invalid dispencer state: {Status}");
            }

            if (!IsActive)
            {
                throw new InvalidOperationException($"Unable to operate with deactivated dispenser");
            }

            Status = DispenserStatus.Reserved;
            ReservedFor = reservedFor;

            var usage = Usage.CreateReserved(Id, amount);
            _usages.Add(usage);
            return usage;
        }


        public static Dispenser CreateNewDispenser(decimal volume, IBeerFlowSettings beerFlowSettings)
        {
            return new Dispenser(Guid.NewGuid(), volume, DispenserStatus.Closed, true, beerFlowSettings);
        }


        public static Dispenser CreateDispenser(
            Guid id,
            decimal volume,
            DispenserStatus status,
            bool isActive,
            IList<Usage> usages = null,
            IBeerFlowSettings beerFlowSettings = null)
        {
            var Dispenser = new Dispenser(id, volume, status, isActive, beerFlowSettings);
            Dispenser.SetUsages(usages);
            return Dispenser;
        }
    }
}