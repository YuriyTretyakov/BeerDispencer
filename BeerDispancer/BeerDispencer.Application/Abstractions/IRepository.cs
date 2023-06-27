using System;
namespace BeerDispencer.Application.Abstractions
{
	public interface IRepository<T> where T: class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(string id);
		Task<T> AddAsync(T dto);
		Task UpdateAsync(T dto);
		Task DeleteAsync(string id);
	}
}

