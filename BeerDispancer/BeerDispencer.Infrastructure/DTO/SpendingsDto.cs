using System;

namespace Beerdispancer.Domain.Entities
{
	public class SpendingsDto
	{
		public IEnumerable<UsageDto> Usages { get; set; }
		public double Amount { get; set; }
    }
}

