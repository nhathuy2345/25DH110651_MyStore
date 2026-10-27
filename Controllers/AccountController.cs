using System;
using System.Linq;
using System.Web.Mvc;
using _25DH110651_MyStore.Models;
using _25DH110651_MyStore.Models.ViewModel; // Nhớ tạo file CustomerVM.cs trước nhé

namespace _25DH110651_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // ================= ĐĂNG KÝ (REGISTER) =================
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // ================= ĐĂNG KÝ (REGISTER) =================
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

        // Sử dụng Transaction để đảm bảo lưu cả 2 bảng hoặc không lưu gì cả
        using (var transaction = db.Database.BeginTransaction())
        {
            try
            {
                // 1. Lưu tài khoản
                var newUser = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    UserRole = "Customer" // Nếu DB yêu cầu độ dài ngắn hơn, hãy sửa thành "KH" hoặc "User"
                };
                db.Users.Add(newUser);
                db.SaveChanges(); // Lỗi có thể văng ra ở đây

                // 2. Lưu thông tin khách hàng
                var newCustomer = new Customer
                {
                    Username = model.Username,
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress
                };
                db.Customers.Add(newCustomer);
                db.SaveChanges(); // Hoặc văng ra ở đây

                // Nếu mọi thứ OK thì xác nhận lưu vào Database
                transaction.Commit();
                return RedirectToAction("Login");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Hủy bỏ mọi thay đổi nếu có lỗi
                transaction.Rollback();

                // Lấy chi tiết lỗi
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.PropertyName + ": " + x.ErrorMessage);
                string fullErrorMessage = string.Join("; ", errorMessages);
                
                // Gửi lỗi ra ngoài View để hiển thị thay vì văng màn hình vàng
                ModelState.AddModelError("", "Lỗi lưu Database: " + fullErrorMessage);
                return View(model);
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

        // ================= ĐĂNG NHẬP (LOGIN) =================
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                Session["Username"] = user.Username;
                return RedirectToAction("Profile");
            }
            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // ================= XEM THÔNG TIN CÁ NHÂN (DETAIL) =================
        public ActionResult Profile()
        {
            if (Session["Username"] == null) return RedirectToAction("Login");

            string username = Session["Username"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);

            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        // ================= CHỈNH SỬA THÔNG TIN (EDIT) =================
        [HttpGet]
        public ActionResult EditProfile()
        {
            if (Session["Username"] == null) return RedirectToAction("Login");

            string username = Session["Username"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);

            if (customer == null) return HttpNotFound();
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