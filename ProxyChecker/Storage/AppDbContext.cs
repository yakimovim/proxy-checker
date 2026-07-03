using Microsoft.EntityFrameworkCore;

namespace ProxyChecker.Storage
{
  internal class AppDbContext : DbContext
  {
    // The DI container passes configurations through this constructor
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Loader> Loaders { get; set; }
  }
}
