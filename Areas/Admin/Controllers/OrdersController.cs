using _25DH110651_MyStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Security.Cryptography.X509Certificates;


namespace _25DH110651_MyStore.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Admin/Orders
        public ActionResult Index(string customerName,string paymentStatus,DateTime? orderDate,int? page)
        {
            {
                var orders = db.Orders.Include("Customer").AsQueryable();

                if (!string.IsNullOrEmpty(customerName))
                {
                    orders = orders.Where(o => o.Customer.CustomerName.Contains(customerName));
                }
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orders = orders.Where(o => o.PaymentStatus == paymentStatus);
                }
                if (orderDate.HasValue)
                {
                    //so sánh ngày ( bỏ qua giơ phút giây)
                    orders = orders.Where(o => o.OrderDate == orderDate.Value);
                }
                //sắp xếp mới nhất lên trên đầu
                orders = orders.OrderByDescending(o => o.OrderDate);
                //phân trang (mỗi trang 5 dòng như trên ảnh)
                int pageSize = 15;
                int pageNumber = (page ?? 1);

                //truyền lại cấc giá trị lọc quua viewbag để giữ trạng thái trên form
                ViewBag.CustomerName = customerName;
                ViewBag.PaymentStatus = paymentStatus;
                ViewBag.OrderDate = orderDate?.ToString("yyyy-MM-dd");

                return View(orders.ToPagedList(pageNumber, pageSize));
            }
            
        }
        public ActionResult DetailedOrder(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            //lấy order,kèm theo customer,kèm theo chi tiết đơn hàng và sản phẩm
            var order = db.Orders
                .Include("Customer")
                .Include("OrderDetails.Product")
                .FirstOrDefault(o => o.OrderID == id);
            if (order == null )
            {
                return HttpNotFound();
            }
            return View(order);
        }
    }
}