using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;   // ⬅️ THÊM DÒNG NÀY (bắt buộc)

namespace _25DH110651_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortOrder { get; set; }
        public IPagedList<Product> Products { get; set; }
    }
}