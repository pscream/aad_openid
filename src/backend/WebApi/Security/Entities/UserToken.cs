using System;

using Microsoft.AspNetCore.Identity;

namespace WebApi.Security.Entities
{

    public class UserToken : IdentityUserToken<Guid>
    {
    }

}
