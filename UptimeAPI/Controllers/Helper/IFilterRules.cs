using System.Linq;

namespace UptimeAPI.Controllers.Helper
{
    public interface IFilterRules
    {
        IQueryable GetFilteredQuery();
        IQueryable GetQuery();
    }
}