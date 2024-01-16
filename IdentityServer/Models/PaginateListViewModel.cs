using System.Collections.Generic;

namespace IdentityServer.Models
{
    public class PaginatedListViewModel<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
    }
}