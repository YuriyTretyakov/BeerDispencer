namespace BeerDispenser.Application.Abstractions
{
    public interface IRepository<T> where T: class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(Guid id);
		Task<T> AddAsync(T dto);
		Task UpdateAsync(T dto);
		Task DeleteAsync(Guid id);
	}
}

