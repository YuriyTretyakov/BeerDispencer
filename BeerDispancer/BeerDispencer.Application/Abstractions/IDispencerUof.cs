using System;
using System.Transactions;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Application.DTO;


namespace BeerDispancer.Application.Abstractions
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

