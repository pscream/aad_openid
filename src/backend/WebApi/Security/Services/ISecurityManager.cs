using System;
using System.Threading.Tasks;

namespace WebApi.Security.Services
{

    public interface ISecurityManager
    {

        Task<string> CreateToken(string userName, string password);

        Task<string> CreateExternalUserToken(string userName);

        Task CreateOrUpdateExternalUser(string username, Guid externalId);

    }

}
