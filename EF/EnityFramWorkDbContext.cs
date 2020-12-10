using CartModuleApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CartModuleApi.EntityFrameWork
{
    public partial class EnityFramWorkDbContext : DbContext
    {
        public EnityFramWorkDbContext(DbContextOptions<EnityFramWorkDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItem>()
                .HasKey(c => new { c.Id });
        }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<CartItem> CartItems { get; set; }
    }
}
