using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nonuso.Infrastructure.Persistence.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
        {
            builder.HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("8821F3D9-FE96-4404-9C2F-8830A043A931"),
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("AA5334F3-837D-4F65-9ECC-8E471DEF97E6"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            },
            new IdentityRole<Guid>
            {
                Id= Guid.Parse("9302B5A3-D93B-4152-BD75-9F9AE7E9FF83"),
                Name = "Business",
                NormalizedName = "BUSINESS",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
        }
    }
}
