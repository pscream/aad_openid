using System;

using Microsoft.AspNetCore.Identity;

namespace WebApi.Security.Entities
{

    public class User : IdentityUser<Guid>
    {

        public bool IsExternal { get; set; }
        public Guid? ExternalId { get; set; }

    }

}
