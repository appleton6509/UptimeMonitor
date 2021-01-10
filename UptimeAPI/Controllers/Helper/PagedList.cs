using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.QueryParams;

namespace UptimeAPI.Controllers.Helper
{
    public class PagedList<T> : List<T>
    {

        public PaginationDTO Pagination { get; private set; }

        public PagedList(List<T> query, PaginationDTO pages)
        {
            Pagination = pages;
            AddRange(query);
        }
        public static PagedList<T> ToPagedList(IQueryable<T> query, PaginationParam pages)
        {
            if (pages.MaxPageSize == 0 || pages.RequestedPage == 0)
                throw new ArgumentException();

            PaginationDTO page = new PaginationDTO();
            page.MaxPageSize = pages.MaxPageSize;
            page.RequestedPage = pages.RequestedPage;
            page.TotalPages = (int)Math.Ceiling((query.Count() / (double)pages.MaxPageSize));
            List<T> list = query.Skip((page.RequestedPage - 1) * page.MaxPageSize).Take(page.MaxPageSize).ToList();
            return new PagedList<T>(list, page);
        }
    }
}
