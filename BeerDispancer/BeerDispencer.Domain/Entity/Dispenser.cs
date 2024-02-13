using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Implementations;
using BeerDispenser.Shared;
using BeerDispenser.Shared.Dto;

namespace BeerDispenser.Domain.Entity
{
    public sealed class Dispenser : Entity
    {
        public decimal Volume { get; private set; }
        public DispenserStatusDto Status { get; private set; }
    
        public bool IsActive { get; private set; }

        private IBeerFlowSettings _beerFlowSettings;
        public IReadOnlyCollection<Usage> Usages => _usages;


        private List<Usage> _usages = new List<Usage>();

        private Dispenser(
            Guid id,
            decimal volume,
            DispenserStatusDto status,
            bool isActive,
            IBeerFlowSettings beerFlowSettings = null)
        {
            Id = id;
            Volume = volume;
            Status = status;
            _beerFlowSettings = beerFlowSettings;
            IsActive = isActive;
        }


        public Usage Open()
        {
            if (Status == DispenserStatusDto.Opened || Status == DispenserStatusDto.OutOfService)
            {
                throw new InvalidOperationException($"Invalid Dispenser state: {Status}");
            }

            if (!IsActive)
            {
                throw new InvalidOperationException($"Unable to operate with deactivated dispenser");
            }

            Status = DispenserStatusDto.Opened;

            var usage = Usage.CreateStarted(Id, _beerFlowSettings);
            _usages.Add(usage);
            return usage;
        }

        public Usage Close()
        {
            if (Status == DispenserStatusDto.Closed || Status == DispenserStatusDto.OutOfService)
            {
                throw new InvalidOperationException($"Invalid Dispenser state: {Status}");
            }

            if (!IsActive)
            {
                throw new InvalidOperationException($"Unable to operate with deactivated dispenser");
            }

            Status = DispenserStatusDto.Closed;

            var currentUsage = GetRecentUsage();
            currentUsage.SetClose();
            return currentUsage;
        }

        public Usage GetRecentUsage()
        {
            var currentUsage = _usages.First(x => x.ClosedAt == null);
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
                    Id = x.Id,
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

        public static Dispenser CreateNewDispenser(decimal volume, IBeerFlowSettings beerFlowSettings)
        {
            return new Dispenser(Guid.NewGuid(), volume, DispenserStatusDto.Closed, true, beerFlowSettings);
        }


        public static Dispenser CreateDispenser(
            Guid id,
            decimal volume,
            DispenserStatusDto status,
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