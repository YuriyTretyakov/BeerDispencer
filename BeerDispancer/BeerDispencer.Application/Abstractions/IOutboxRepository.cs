using BeerDispenser.Application.DTO;


namespace BeerDispenser.Application.Abstractions
{
    public interface IOutboxRepository: IRepository<OutboxDto>
	{
        Task<IEnumerable<OutboxDto>> GetNotProccessedEvents();
	}
}

