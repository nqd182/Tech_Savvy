using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using TechSavvy.Models;
using TechSavvy.Models.ViewModels;
using TechSavvy.Repository;

namespace TechSavvy.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            
            // Nhận shipping giá từ cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;

            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),
                ShippingCost = shippingPrice
            };  
            return View(cartVM);
        }
        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }

        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id); // check san pham co trong gio hang chua
            if(cartItem == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItem.Quantity += 1;            
                    
            }
            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Thêm sản phẩm thành công";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if(cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(c => c.ProductId == Id);
            }

            if(cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");

            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);

            }
            TempData["success"] = "Giảm số lượng sản phẩm thành công";

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Increase(int Id)
        {
            ProductModel product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity >= 1 && product.Quantity > cartItem.Quantity)
            {
                ++cartItem.Quantity;
                TempData["success"] = "Tăng số lượng sản phẩm thành công";

            }
            else
            {
                cartItem.Quantity = product.Quantity ?? 0;

                TempData["error"] = "Số lượng sản phẩm trong kho không đủ";
            }
           

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");

            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.ProductId == Id);
            if(cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");

            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Xóa sản phẩm thành công";
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Clear(int Id)
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Xóa giỏ hàng thành công";
            return RedirectToAction("Index");

        }
        // tính phí shipping sử dụng cookies
        [HttpPost]
        [Route("Cart/GetShipping")]
        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string quan, string tinh, string phuong)
        {

            var existingShipping = await _dataContext.Shippings
                .FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);  

            decimal shippingPrice = 0; // Set mặc định giá tiền

            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                //Set mặc định phí ship nếu ko tìm thấy
                shippingPrice = 50000;
            }
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice); // chuyển đổi thành chuỗi JSON
            try
            {
                var cookieOptions = new CookieOptions // tạo cookie
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true // using HTTPS
                };
                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions); // đẩy shippingPriceJson vào cookie với tên là "ShippingPrice"
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error adding shipping price cookie: {ex.Message}");
            }
            return Json(new { shippingPrice });
        }
        [HttpGet]
        [Route("Cart/DeleteShipping")]
        public IActionResult RemoveShippingCookie()
        {
            Response.Cookies.Delete("ShippingPrice");
            return RedirectToAction("Index", "Cart");
        }
    
}
}
    