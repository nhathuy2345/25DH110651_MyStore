using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using _25DH110651_MyStore.Models;
using _25DH110651_MyStore.Models.ViewModel;

namespace _25DH110651_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        [HttpGet]
        public ActionResult Register() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(CustomerVM model)
        {
            if (ModelState.IsValid)
            {
                var checkUser = db.Users.FirstOrDefault(u => u.Username == model.Username);
                if (checkUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại!");
                    return View(model);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var newUser = new User { Username = model.Username, Password = model.Password, UserRole = "Customer" };
                        db.Users.Add(newUser);
                        db.SaveChanges();

                        var newCustomer = new Customer
                        {
                            Username = model.Username,
                            CustomerName = model.CustomerName,
                            CustomerEmail = model.CustomerEmail,
                            CustomerPhone = model.CustomerPhone,
                            CustomerAddress = model.CustomerAddress
                        };
                        db.Customers.Add(newCustomer);
                        db.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction("Login");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                        return View(model);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Login() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                Session["Username"] = user.Username;
                FormsAuthentication.SetAuthCookie(user.Username, true);
                return RedirectToAction("Profile");
            }
            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login");
            string username = User.Identity.Name;
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null) return HttpNotFound();

            var pmmodel = new PersonalMenuVM();
            pmmodel.IsLoggedIn = true;
            pmmodel.Username = username;
            var cart = Session["Cart"] as Cart;
            pmmodel.CartCount = (cart != null) ? cart.Items.Count() : 0;
            ViewBag.pmmodel = pmmodel;

            return View(customer);
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login");
            string username = User.Identity.Name;
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null) return HttpNotFound();

            var pmmodel = new PersonalMenuVM();
            pmmodel.IsLoggedIn = true;
            pmmodel.Username = username;
            var cart = Session["Cart"] as Cart;
            pmmodel.CartCount = (cart != null) ? cart.Items.Count() : 0;
            ViewBag.pmmodel = pmmodel;

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(Customer model)
        {
            if (ModelState.IsValid)
            {
                var customer = db.Customers.FirstOrDefault(c => c.CustomerID == model.CustomerID);
                if (customer != null)
                {
                    customer.CustomerName = model.CustomerName;
                    customer.CustomerEmail = model.CustomerEmail;
                    customer.CustomerPhone = model.CustomerPhone;
                    customer.CustomerAddress = model.CustomerAddress;
                    db.SaveChanges();
                    return RedirectToAction("Profile");
                }
            }
            return View(model);
        }
    }
}