using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApi.Security.Entities;

namespace WebApi.Security.Contexts.Configs
{

    internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {

        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("IdentUserRoles");
        }

    }

}
