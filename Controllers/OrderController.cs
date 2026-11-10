using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using _25DH110651_MyStore.Models;
using _25DH110651_MyStore.Models.ViewModel;

namespace _25DH110651_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as Cart;
            if (cart == null || !cart.Items.Any()) return RedirectToAction("Index", "Home");
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var user = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            var customer = db.Customers.FirstOrDefault(c => c.Username == user.Username);
            if (customer == null) return RedirectToAction("Login", "Account");

            var model = new CheckoutVM
            {
                CartItems = cart.Items.ToList(),
                CustomerID = customer.CustomerID,
                Username = customer.Username,
                ShippingAddress = customer.CustomerAddress,
                OrderDate = DateTime.Now,
                TotalAmount = cart.TotalValue(),
                PaymentStatus = "Pending"
            };

            var pmmodel = new PersonalMenuVM();
            pmmodel.IsLoggedIn = User.Identity.IsAuthenticated;
            pmmodel.Username = User.Identity.Name;
            pmmodel.CartCount = cart.Items.Count();
            ViewBag.pmmodel = pmmodel;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            var cart = Session["Cart"] as Cart;
            if (cart == null || !cart.Items.Any()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                string username = User.Identity.Name;
                var customer = db.Customers.FirstOrDefault(c => c.Username == username);
                if (customer == null) return RedirectToAction("Login", "Account");

                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = DateTime.Now,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = "Pending",
                    PaymentMethod = model.PaymentMethod,
                    DeliveryMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress
                };
                db.Orders.Add(order);
                db.SaveChanges();

                foreach (var item in cart.Items)
                {
                    var orderDetail = new OrderDetail { OrderID = order.OrderID, ProductID = item.ProductID, Quantity = item.Quantity, UnitPrice = item.UnitPrice };
                    db.OrderDetails.Add(orderDetail);
                }
                db.SaveChanges();
                Session["Cart"] = null;
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            return View(model);
        }

        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders.Include("OrderDetails").Include("OrderDetails.Product").SingleOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();

            var pmmodel = new PersonalMenuVM();
            pmmodel.IsLoggedIn = User.Identity.IsAuthenticated;
            pmmodel.Username = User.Identity.Name;
            var cart = Session["Cart"] as Cart;
            pmmodel.CartCount = (cart != null) ? cart.Items.Count() : 0;
            ViewBag.pmmodel = pmmodel;
            return View(order);
        }

        public ActionResult MyOrder()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
            string username = User.Identity.Name;
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null) return HttpNotFound();

            var orders = db.Orders.Include(o => o.OrderDetails.Select(od => od.Product)).Where(o => o.CustomerID == customer.CustomerID).OrderByDescending(o => o.OrderDate).ToList();

            var pmmodel = new PersonalMenuVM();
            pmmodel.IsLoggedIn = true;
            pmmodel.Username = username;
            var cart = Session["Cart"] as Cart;
            pmmodel.CartCount = (cart != null) ? cart.Items.Count() : 0;
            ViewBag.pmmodel = pmmodel;

            return View(orders);
        }
    }
}