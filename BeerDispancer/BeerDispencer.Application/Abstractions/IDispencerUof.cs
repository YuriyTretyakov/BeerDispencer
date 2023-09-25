using System.Transactions;
using BeerDispenser.Application.Abstractions;


namespace BeerDispenser.Application.Abstractions
{
    public interface IDispencerUof : IDisposable
	{
        IDispencerRepository DispencerRepo { get; set; }
		IUsageRepository UsageRepo { get; set; }
		Task Complete();
        public TransactionScope StartTransaction();
        public void CommitTransaction();
    }
}

