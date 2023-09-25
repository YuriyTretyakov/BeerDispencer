using Microsoft.EntityFrameworkCore;

namespace BeerDispenser.Infrastructure.Authorization
{
    public interface ILoginDbContext
    {
        DbSet<Token> Tokens { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}