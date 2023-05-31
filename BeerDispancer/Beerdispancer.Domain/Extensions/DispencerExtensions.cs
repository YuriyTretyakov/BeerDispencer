using System;
using Beerdispancer.Domain.Entities;

namespace Beerdispancer.Domain.Extensions
{
	public static class DispencerExtensions
	{
		public static DispencerDto ToDto(this DispencerDto model)
		{
			return new DispencerDto { Volume = model.Volume, Status = DispencerStatusDto.Close };
		}
    }
}

