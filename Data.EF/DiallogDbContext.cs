using Data.EF.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Data.EF
{
    public class DiallogDbContext : DbContext
    {
        public DbSet<RoleToPermissions> RolesToPermissions { get; set; }

        public DiallogDbContext(DbContextOptions<DiallogDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RoleToPermissionsEntityConfiguration());
        }
    }
}
