using System.Linq;
using System.Web.Mvc;
using _25DH110651_MyStore.Models;
using _25DH110651_MyStore.Models.ViewModel;
using System.Collections.Generic;

namespace _25DH110651_MyStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // 1. TRANG THỐNG KÊ (BẢNG)
        public ActionResult Index()
        {
            // Dùng LINQ để nhóm sản phẩm theo Danh mục và tính toán
            var statistics = db.Products
                .GroupBy(p => p.Category.CategoryName)
                .Select(g => new CategoryStatisticVM
                {
                    CategoryName = g.Key,
                    Quantity = g.Count(),
                    MaxPrice = g.Max(p => p.ProductPrice),
                    MinPrice = g.Min(p => p.ProductPrice),
                    AvgPrice = g.Average(p => p.ProductPrice)
                }).ToList();

            return View(statistics);
        }

        // 2. TRANG BIỂU ĐỒ
        public ActionResult Charts()
        {
            return View();
        }

        // 3. API TRẢ VỀ DỮ LIỆU JSON CHO GOOGLE CHART
        public JsonResult GetChartData()
        {
            var data = db.Products
                .GroupBy(p => p.Category.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    Quantity = g.Count()
                }).ToList();

            // Định dạng lại mảng cho Google Chart: [['Danh mục', 'Số lượng'], ['Truyện tranh', 10], ...]
            var chartData = new List<object>();
            chartData.Add(new object[] { "Danh mục", "Số lượng" });

            foreach (var item in data)
            {
                chartData.Add(new object[] { item.CategoryName, item.Quantity });
            }

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}