using Microsoft.EntityFrameworkCore;

namespace lowishBackend_v2.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Gift> Gifts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи между User и Wishlist (один ко многим)
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи между Wishlist и Gift (один ко многим)
            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Wishlist)
                .WithMany(w => w.Gifts)
                .HasForeignKey(g => g.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}