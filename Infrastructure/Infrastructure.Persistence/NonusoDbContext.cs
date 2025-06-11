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

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());

            //modelBuilder.Entity<Product>()
            //    .HasOne(x => x.Category)
            //    .WithMany()
            //    .HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(x => x.UserId);
                entity.HasIndex(x => x.CategoryId);

                entity.HasGeneratedTsVectorColumn(
                    p => p.SearchVector,
                    "simple",
                    p => new { p.Title, p.Description })
                    .HasIndex(p => p.SearchVector)
                    .HasMethod("GIN");

                entity.HasIndex(p => p.Location).HasMethod("GIST");

            });

            modelBuilder.Entity<ProductHistory>()
              .HasOne(x => x.User)
              .WithMany()
              .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Favorite>()
             .HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Favorite>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<LastSearch>()
             .HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId);

            //modelBuilder.Entity<Favorite>().HasIndex("OwnerId");
            //modelBuilder.Entity<Product>().HasIndex("OwnerId");
            //modelBuilder.Entity<ProductHistory>().HasIndex("UserId");

            //modelBuilder.ApplyConfiguration(new RoleConfiguration());

        }
    }
}
