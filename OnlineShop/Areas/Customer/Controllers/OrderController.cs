using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Get Checkout
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetString("useid");
            var cookieValue = Request.Cookies["log"];
            if (userId == null && cookieValue == null)
            {
                return View();
            }
            string decode = AesOperation.Base64Decode(cookieValue);
            var userdata = _db.ApplicationUsers.FirstOrDefault(c=>c.Id == userId);
            Order order = new Order();
            order.Name= $"{userdata.FristName} {userdata.LastName}";
            order.Email= cookieValue;
            order.Address = userdata.Address;
            order.PhoneNo = userdata.PhoneNumber;
            return View(order);
        }
        //POST Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order anOrder)
        {
            anOrder.OrderDate= DateTime.Now;
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
                foreach(var product in products)
                {
                    OrderDetails orderDetails= new OrderDetails();
                    orderDetails.ProductId = product.Id;
                    anOrder.OrderDetails.Add(orderDetails);
                }
            }
            anOrder.OrderNo=GetOrderNo();
            _db.Orders.Add(anOrder);
            await _db.SaveChangesAsync();
            HttpContext.Session.Set("products",new List<Products>());
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        public string GetOrderNo()
        {
            int rowCount = _db.Orders.ToList().Count()+1;
            return rowCount.ToString("000");
        }
    }
}
