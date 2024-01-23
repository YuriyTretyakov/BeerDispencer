using System.Collections.ObjectModel;
using BeerDispenser.Application.DTO;

namespace BeerDispenser.Application.Abstractions
{
    public interface IPaymentCardRepository: IRepository<PaymentCardDto>
	{
		public Task<ReadOnlyCollection<PaymentCardDto>> GetUserCards(Guid userId);
		public Task<PaymentCardDto> GetDefaultCard(Guid userId);
    }
}

