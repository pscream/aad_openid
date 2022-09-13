using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApi.Security.Entities;

namespace WebApi.Security.Contexts.Configs
{

    internal class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {

        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable("IdentRoleClaims");
        }

    }

}
