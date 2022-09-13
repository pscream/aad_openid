using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApi.Security.Entities;

namespace WebApi.Security.Contexts.Configs
{

    internal class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {

        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.ToTable("IdentUserClaims");
        }

    }

}
