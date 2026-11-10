using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _25DH110651_MyStore.Models;
using _25DH110651_MyStore.Models.ViewModel;
using PagedList;

namespace _25DH110651_MyStore.Controllers
{
    public class HomeController : Controller
    {
        // Khởi tạo kết nối CSDL
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Home/Index
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVm();
            var products = db.Products.AsQueryable();

            // 1. Lọc sản phẩm theo tiêu chí tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDecription.Contains(searchTerm) || // ĐÃ SỬA LỖI CHÍNH TẢ (thêm chữ s)
                    p.Category.CategoryName.Contains(searchTerm));
            }

            // 2. Xử lý phân trang 
            int pageNumber = page ?? 1; // Lấy trang hiện tại, nếu không có mặc định là trang 1
            int pageSize = model.PageSize; // Lấy pageSize từ ViewModel (bạn đã set là 6)

            // 3. Lấy Top 10 sản phẩm bán chạy nhất (Featured)
            // Dùng Sum để tính tổng số lượng đã bán, tránh lỗi null nếu SP chưa có ai mua
            model.FeaturedProducts = products
                .OrderByDescending(p => p.OrderDetails.Sum(od => (int?)od.Quantity) ?? 0)
                .Take(10)
                .ToList();

            // 4. Lấy danh sách sản phẩm ít người mua nhất (NewestProducts) VÀ PHÂN TRANG
            model.NewProducts = products
                .OrderBy(p => p.OrderDetails.Sum(od => (int?)od.Quantity) ?? 0)
                .ToPagedList(pageNumber, pageSize);

            // --- THÊM ĐOẠN NÀY ĐỂ HIỂN THỊ MENU TÀI KHOẢN & GIỎ HÀNG ---
            var pmmodel = new PersonalMenuVM();
            pmmodel.IsLoggedIn = User.Identity.IsAuthenticated;
            if (pmmodel.IsLoggedIn) pmmodel.Username = User.Identity.Name;
            var cart = Session["Cart"] as Cart;
            pmmodel.CartCount = (cart != null) ? cart.Items.Count() : 0;
            ViewBag.pmmodel = pmmodel;
            // -------------------------------------------------------------

            

            return View(model);
        }

        // GET: Home/ProductDetails/5
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            // Lấy sản phẩm cùng danh mục (trừ sản phẩm hiện tại)
            var products = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();

            ProductDetailsVM model = new ProductDetailsVM();
            model.product = pro;

            // Phân trang cho TopProducts
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;

            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToList();

            // Sắp xếp top sản phẩm bán chạy cùng danh mục
            model.TopProducts = products
                .OrderByDescending(p => p.OrderDetails.Sum(od => (int?)od.Quantity) ?? 0)
                .ToPagedList(pageNumber, pageSize);

            // Xử lý số lượng mua
            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }

            // Tính tiền tạm tính
            model.estimatedValue = model.quantity * model.product.ProductPrice;

            return View(model);
        }
        [ChildActionOnly]
        public ActionResult MenuCategories()
        {
            var categories = db.Categories.ToList();
            return PartialView("_CategoriesPV", categories);
        }
        public ActionResult ProductList(int? categoryId)
        {
            var products = db.Products.AsQueryable();
            if (categoryId.HasValue) products = products.Where(p => p.CategoryID == categoryId.Value);

            var pmmodel = new PersonalMenuVM();
            pmmodel.IsLoggedIn = User.Identity.IsAuthenticated;
            pmmodel.Username = User.Identity.Name;
            var cart = Session["Cart"] as Cart;
            pmmodel.CartCount = (cart != null) ? cart.Items.Count() : 0;
            ViewBag.pmmodel = pmmodel;

            return View(products.ToList());
        }
        public ActionResult ProductDetail(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Product pro = db.Products.Find(id);
            if (pro == null) return HttpNotFound();

            var products = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();
            ProductDetailsVM model = new ProductDetailsVM();
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToList();
            model.TopProducts = products.OrderByDescending(p => p.ProductID).Take(8).ToPagedList(1, 8);

            var pmmodel = new PersonalMenuVM();
            pmmodel.IsLoggedIn = User.Identity.IsAuthenticated;
            pmmodel.Username = User.Identity.Name;
            var cart = Session["Cart"] as Cart;
            pmmodel.CartCount = (cart != null) ? cart.Items.Count() : 0;
            ViewBag.pmmodel = pmmodel;

            return View(model);
        }
    }
}