using CarRental.Repository.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Repository
{
    public class DiallogDbContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DiallogDbContext(DbContextOptions<DiallogDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookingEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CarEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentEntityConfiguration());
        }
    }
}
