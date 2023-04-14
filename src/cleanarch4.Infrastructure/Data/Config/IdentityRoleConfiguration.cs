using cleanarch4.Core.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cleanarch4.Infrastructure.Data.Config;

public class IdentityRoleConfiguration : IEntityTypeConfiguration<Microsoft.AspNetCore.Identity.IdentityRole>
{
  public void Configure(EntityTypeBuilder<Microsoft.AspNetCore.Identity.IdentityRole> builder)
  {
    builder.Property(p => p.Id)
        .HasMaxLength(128)
        .IsRequired();
  }
}
