using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _25DH110651_MyStore.Models;

namespace _25DH110651_MyStore.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // 1. Xem danh sách khách hàng (List)
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.User).ToList();
            return View(customers);
        }

        // 2. Xem chi tiết thông tin khách hàng (Detail)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Include(c => c.User).FirstOrDefault(c => c.CustomerID == id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // Nếu Admin muốn xóa khách hàng (tùy chọn)
        [HttpPost]
        public ActionResult DeleteSelected(List<int> selectedIds)
        {
            if (selectedIds != null && selectedIds.Count > 0)
            {
                foreach (var id in selectedIds)
                {
                    var customer = db.Customers.Find(id);
                    if (customer != null)
                    {
                        // Xóa tài khoản User liên kết (nếu có)
                        var user = db.Users.Find(customer.Username);
                        if (user != null) db.Users.Remove(user);

                        db.Customers.Remove(customer);
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}