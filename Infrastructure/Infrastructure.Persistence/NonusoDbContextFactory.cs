using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nonuso.Infrastructure.Persistence
{
    public class NonusoDbContextFactory : IDesignTimeDbContextFactory<NonusoDbContext>
    {
        public NonusoDbContext CreateDbContext(string[] args)
        {

            var connection = File.ReadAllText("/run/secrets/db_connection").Trim();

            var optionsBuilder = new DbContextOptionsBuilder<NonusoDbContext>();
            optionsBuilder.UseNpgsql(connection, npgsqlOptions => npgsqlOptions.UseNetTopologySuite());

            return new NonusoDbContext(optionsBuilder.Options);
        }
    }
}
