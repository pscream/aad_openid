using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApi.Security.Entities;

namespace WebApi.Security.Contexts.Configs
{

    internal class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {

        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable("IdentUserTokens");
        }

    }

}
