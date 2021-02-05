using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Repository.EntityConfigurations
{
    class BookingEntityConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.GuestUserId).IsRequired();
            builder.Property(o => o.GuestName).IsRequired();
            builder.Property(o => o.ReferenceNumber).IsRequired();
            builder.HasIndex(o => o.GuestUserId);
            builder.HasIndex(o => o.GuestName);
            builder.HasIndex(o => o.ReferenceNumber);
            builder.HasIndex(o => o.Status);
            builder.Property(o => o.TotalCost).HasPrecision(18, 6);

            builder.HasOne(o => o.Car).WithMany();
            builder.HasMany(o => o.Payments).WithOne(o => o.Booking).Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
