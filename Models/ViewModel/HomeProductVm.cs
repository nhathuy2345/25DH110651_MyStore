using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using _25DH110651_MyStore.Models;
namespace _25DH110651_MyStore.Models.ViewModel
{
    public class HomeProductVm
    {
        // tiêu chí search
        public string SearchTerm { get; set; }

        // thuộc tính để hỗ trợ phân trang
        public int PageNumber { get; set; } // Mặc định trang đầu tiên
        public int PageSize { get; set; } = 10; // Số lượng sản phẩm trên mỗi trang
                                                // danh sách sản phẩm nổi bật
        public List<Product> FeaturedProducts { get; set; }

        //danh sách sản phẩm mới nhất để phân trang
        public IPagedList<Product> NewProducts { get; set; }
    }
}