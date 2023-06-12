using System;
namespace BeerDispencer.Application.Abstractions
{
	public interface IRepository<T, TIdType> where T: class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(TIdType id);
		Task<T> AddAsync(T dto);
		Task UpdateAsync(T dto);
		Task DeleteAsync(TIdType id);
	}
}

