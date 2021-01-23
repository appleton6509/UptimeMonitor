using Data.Models;
using Data.Repositories;

namespace UptimeAPI.Controllers.Repositories
{
    public interface IWebUserRepository : IRepository<WebUser>
    {
        WebUser Get(string identityId);
    }
}