using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Repository.EntityConfigurations
{
    class PaymentEntityConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(o => o.PaymentId);
            builder.Property(o => o.PaymentId).ValueGeneratedOnAdd();
            builder.Property(o => o.Amount).HasPrecision(18, 6);
        }
    }
}
