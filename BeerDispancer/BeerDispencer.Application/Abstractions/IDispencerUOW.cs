using System.Transactions;
using BeerDispencer.Application.Abstractions;


namespace BeerDispancer.Application.Abstractions
{
    public interface IDispencerUOW : IDisposable
	{
        IDispencerRepository DispencerRepo { get; set; }
		IUsageRepository UsageRepo { get; set; }
		Task Complete();
        public TransactionScope StartTransaction();
        public void CommitTransaction();
        public Task ProcessPaymentAsync(Guid dispencerId, double amount);
    }
}

