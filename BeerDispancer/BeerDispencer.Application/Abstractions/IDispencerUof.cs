using BeerDispencer.Application.Abstractions;


namespace BeerDispancer.Application.Abstractions
{
    public interface IDispencerUof : IDisposable
	{
        IDispencerRepository DispencerRepo { get; set; }
		IUsageRepository UsageRepo { get; set; }
    }
}

