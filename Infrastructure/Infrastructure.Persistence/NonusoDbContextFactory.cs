using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
