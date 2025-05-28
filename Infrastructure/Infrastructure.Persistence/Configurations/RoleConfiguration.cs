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
                ConcurrencyStamp = "3E55B188-F822-4BDE-B1CC-96BF79C74797",
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("AA5334F3-837D-4F65-9ECC-8E471DEF97E6"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "937118E8-A1EA-41DD-8FE7-F47CBDC27FB2",
            },
            new IdentityRole<Guid>
            {
                Id= Guid.Parse("9302B5A3-D93B-4152-BD75-9F9AE7E9FF83"),
                Name = "Business",
                NormalizedName = "BUSINESS",
                ConcurrencyStamp = "C2A9E755-4A7F-46F2-B36E-364E86DF6DEC"
            });
        }
    }
}
