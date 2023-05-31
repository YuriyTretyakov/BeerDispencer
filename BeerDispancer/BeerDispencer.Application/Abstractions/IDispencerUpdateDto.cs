using Beerdispancer.Domain.Entities;

namespace BeerDispancer.Application.Abstractions
{
    public interface IDispencerUpdate
    {
        Guid Id { get; set; }
        DispencerStatusDto Status { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}