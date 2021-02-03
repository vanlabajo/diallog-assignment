using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EF.EntityConfigurations
{
    class RoleToPermissionsEntityConfiguration : IEntityTypeConfiguration<RoleToPermissions>
    {
        public void Configure(EntityTypeBuilder<RoleToPermissions> builder)
        {
            builder.Property("_permissionsInRole")
                .HasColumnName("PermissionsInRole");
        }
    }
}
