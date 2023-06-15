using Microsoft.EntityFrameworkCore;

namespace BeerDispencer.Infrastructure.Authorization
{
    public interface ILoginDbContext
    {
        DbSet<Token> Tokens { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}