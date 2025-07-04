using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Infrastructure.Persistence.Configurations;

namespace Nonuso.Infrastructure.Persistence
{
    public class NonusoDbContext(DbContextOptions<NonusoDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        #region DBSET

        public DbSet<Category> Category { get; set; }
        public DbSet<Favorite> Favorite { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductReport> ProductReport { get; set; }
        public DbSet<ProductHistory> ProductHistory { get; set; }
        public DbSet<LastSearch> LastSearch { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<ProductRequest> ProductRequest { get; set; }
        public DbSet<Conversation> Conversation { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<ConversationInfo> ConversationInfo { get; set; }
        public DbSet<UserBlock> UserBlock { get; set; }
        public DbSet<Review> Review { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());

            modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.ReviewerUserId, r.ReviewedUserId, r.ProductRequestId })
            .IsUnique();

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasGeneratedTsVectorColumn(
                    p => p.SearchVector,
                    "simple",
                    p => new { p.Title, p.Description })
                    .HasIndex(p => p.SearchVector)
                    .HasMethod("GIN");

                entity.HasIndex(p => p.Location).HasMethod("GIST");

            });
        }
    }
}
