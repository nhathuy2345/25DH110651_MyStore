using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using _25DH110651_MyStore.Models;
using _25DH110651_MyStore.Models.ViewModel;

namespace _25DH110651_MyStore.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        public ActionResult Index(int? page)
        {
            var cart = GetCartService().GetCart();
            var products = db.Products.ToList();
            var similarProducts = new List<Product>();

            if (cart.Items != null && cart.Items.Any())
            {
                similarProducts = products.Where(p => cart.Items.Any(ci => ci.Category == p.Category.CategoryName)
                                                && !cart.Items.Any(ci => ci.ProductID == p.ProductID)).ToList();
            }

            int pageNumber = page ?? 1;
            int pageSize = cart.PageSize;
            cart.SimilarProducts = similarProducts.OrderBy(p => p.ProductID).ToPagedList(pageNumber, pageSize);

            var pmmodel = new PersonalMenuVM();
            pmmodel.IsLoggedIn = User.Identity.IsAuthenticated;
            pmmodel.Username = User.Identity.Name;
            pmmodel.CartCount = cart.Items.Count();
            ViewBag.pmmodel = pmmodel;

            return View(cart);
        }

        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cart = GetCartService().GetCart();
                cart.AddItem(product.ProductID, product.ProductName, product.ProductImage, product.ProductPrice, quantity, product.Category.CategoryName);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCartService().GetCart();
            cart.RemoveItem(id);
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCartService().GetCart();
            if (quantity > 0) cart.UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
    }
}