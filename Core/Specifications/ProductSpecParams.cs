using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductSpecParams
    {
        private const int MaxPageSize = 50;
        //PageIndex is page number
        public int PageIndex { get; set; } = 1;
        //pagesize is maximum number of records per page
        private int _pageSize = 6;
        public int PageSize
        {
            get => _pageSize; 
            set => _pageSize = (value>MaxPageSize)?MaxPageSize:value;
        }
        //Filter by brandId and TypeID
        public int? BrandId { get; set; }
        public int? TypeId { get;set; }
        //Sort field
        public string Sort { get; set; }
        //Search field
        private string _search;
        public string Search
        {
            get => _search; 
            set => _search = value.ToLower();
        }
    }
}
