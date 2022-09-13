using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApi.Security.Entities;

namespace WebApi.Security.Contexts.Configs
{

    internal class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {

        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("IdentUserLogins");
        }

    }

}
