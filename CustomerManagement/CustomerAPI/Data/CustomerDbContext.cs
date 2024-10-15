using CustomerAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerAPI.Data
{
    public class CustomerDbContext : DbContext
    {
        public DbSet<CustomerEnt> Customers { get; set; }

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerEnt>(entity =>
            {
                entity.HasKey(c => c.Id); 

                entity.Property(c => c.FirstName)
                    .IsRequired(); 

                entity.Property(c => c.LastName)
                    .IsRequired(); 

                entity.Property(c => c.Email)
                    .IsRequired() 
                    .HasMaxLength(100);

                entity.Property(c => c.Address)
                    .IsRequired(); 

                entity.Property(c => c.MobileNo)
                    .HasMaxLength(15)
                    .IsRequired(false);  

                entity.HasIndex(c => c.Email)
                    .IsUnique(); 
            });
        }
    }
}
