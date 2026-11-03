using _25DH110651_MyStore.Models;
using PagedList;
using System.Collections.Generic;

namespace _25DH110651_MyStore.Models.ViewModel
{
    public class ProductDetailsVM
    {
        public Product product { get; set; }
        public int quantity { get; set; } = 1;
        public decimal estimatedValue { get; set; } // quantity * product.ProductPrice

        // Hỗ trợ phân trang cho TopProducts
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 3;

        // Sản phẩm cùng danh mục
        public List<Product> RelatedProducts { get; set; }

        // Sản phẩm bán chạy cùng danh mục (có phân trang)
        public IPagedList<Product> TopProducts { get; set; }
    }
}