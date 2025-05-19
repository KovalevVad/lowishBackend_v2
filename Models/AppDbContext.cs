using Microsoft.EntityFrameworkCore;

namespace lowishBackend_v2.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Gift> Gifts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка отношения User - WishList (один ко многим)
            modelBuilder.Entity<WishList>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка отношения WishList - Gift (один ко многим)
            modelBuilder.Entity<Gift>()
                .HasOne(g => g.WishList)
                .WithMany()
                .HasForeignKey(g => g.WishListId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}