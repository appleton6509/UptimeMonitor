using Data.Repositories;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.QueryParams;

namespace UptimeAPI.Controllers.Helper
{
    /// <summary>
    /// a helper class for creating lists based on pagination parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>
    {

        public PaginationDTO Pagination { get; private set; }

        public PagedList(List<T> query, PaginationDTO pages)
        {
            Pagination = pages;
            AddRange(query);
        }

        /// <summary>
        /// convert a IQueryable to a paged list
        /// </summary>
        /// <param name="query">the queriable object that will be converted to a paged list</param>
        /// <param name="pages">pagination parameters object</param>
        /// <returns></returns>
        public static PagedList<T> ToPagedList(IFilterRules filter, PaginationParam pages)
        {
           var query = (IQueryable < T >)filter.GetFilteredQuery();

            if (pages.MaxPageSize == 0 || pages.RequestedPage == 0)
                throw new ArgumentException("Missing pagination parameters");

            PaginationDTO page = new PaginationDTO
            {
                MaxPageSize = pages.MaxPageSize,
                RequestedPage = pages.RequestedPage,
                TotalPages = (int)Math.Ceiling((query.Count() / (double)pages.MaxPageSize))
            };
            List<T> list = 
                query.Skip((page.RequestedPage - 1) * page.MaxPageSize)
                .Take(page.MaxPageSize)
                .ToList();
            return new PagedList<T>(list, page);
        }
        public void AddPaginationToResponse(HttpResponse httpResponse )
        {
            httpResponse.Headers.Add("X-Pagination", JsonConvert.SerializeObject(this.Pagination));
        }
    }
}
