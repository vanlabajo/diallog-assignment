using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Repository.EntityConfigurations
{
    class CarEntityConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.Type).IsRequired();
            builder.Property(o => o.GasConsumption).IsRequired();
            builder.Property(o => o.DailyRentalCost).HasPrecision(18, 6);

            builder.Property(o => o.RentedOut).HasField("_rentedOut").UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
        }
    }
}
