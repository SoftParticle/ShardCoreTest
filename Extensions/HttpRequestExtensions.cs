using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.Extensions
{
    public static class HttpRequestExtensions
    {
        public static (string draw, string sortColumn, string sortDirection, string searchTerm, int page, int pageSize)
            GetDataTableParameters(this HttpRequest request)
        {
            var draw = request.Form["draw"].FirstOrDefault();

            // Skip number of Rows count  
            var start = request.Form["start"].FirstOrDefault();

            // Paging Length 10,20  
            var length = request.Form["length"].FirstOrDefault();

            // Sort Column Name  
            var sortColumn = request.Form["columns[" + request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

            // Sort Column Direction (asc, desc)  
            var sortDirection = request.Form["order[0][dir]"].FirstOrDefault();

            // Search Value from (Search box)  
            var searchTerm = request.Form["search[value]"].FirstOrDefault();

            searchTerm = string.IsNullOrEmpty(searchTerm) ? null : searchTerm;

            //Paging Size (10, 20, 50,100)  
            int pageSize = length != null ? Convert.ToInt32(length) : 0;

            int skip = start != null ? Convert.ToInt32(start) : 0;

            int page = skip / pageSize;

            return (draw, sortColumn, sortDirection, searchTerm, page, pageSize);
        }
    }
}
