﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
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
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),

            };

            return View(cartVM);
        }

        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id); // tìm kiếm sản phẩm trong giỏ hàng
            if (cartItem == null)
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
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(c => c.ProductId == Id);
            }

            if (cart.Count == 0)
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
            if (cart.Count == 0)
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
            Response.Cookies.Delete("ShippingPrice");
            Response.Cookies.Delete("CouponTitle");
            Response.Cookies.Delete("ShippingAddress");
            Response.Cookies.Delete("CouponPrice");

            TempData["success"] = "Xóa giỏ hàng thành công";
            return RedirectToAction("Index");

        }
        // tính phí shipping sử dụng cookies
        [HttpPost]
        [Route("Cart/GetShipping")]
        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string quan, string tinh, string phuong, string diachi)
        {
            var existingShipping = await _dataContext.Shippings
                .FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

            decimal shippingPrice = 0;

            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                //Set mặc định phí ship nếu ko tìm thấy
                shippingPrice = 15000;
            }
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice); // chuyển đổi thành chuỗi JSON
            var fullAddress = $"{diachi}, {phuong}, {quan}, {tinh}";
            try
            {
                var cookieOptions = new CookieOptions // tạo cookie
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true, // using HTTPS
                    SameSite = SameSiteMode.Lax
                };
                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions); // đẩy shippingPriceJson vào cookie với tên là "ShippingPrice"
                Response.Cookies.Append("ShippingAddress", fullAddress, cookieOptions);
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
        // áp dụng coupon
        [HttpPost]
        [Route("Cart/GetCoupon")]
        public async Task<IActionResult> GetCoupon(CouponModel couponModel, string coupon_value)
        {
            var validCoupon = await _dataContext.Coupons
                .FirstOrDefaultAsync(x => x.Name == coupon_value);
            if (validCoupon == null)
            {
                return Ok(new { success = false, message = "Mã giảm giá không tồn tại" });
            }
            decimal couponPrice = 0;
            if (validCoupon != null)
            {
                couponPrice = validCoupon.Price;
            }
            string couponTitle = validCoupon.Name + " | " + validCoupon?.Description;

            if (couponTitle != null)
            {
                TimeSpan remainingTime = validCoupon.DateExpired - DateTime.Now;
                int daysRemaining = remainingTime.Days;

                if (daysRemaining >= 0 && validCoupon.Quantity > 0)
                {
                    try
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Lax // Kiểm tra tính tương thích trình duyệt
                        };

                        Response.Cookies.Append("CouponTitle", couponTitle, cookieOptions);
                        Response.Cookies.Append("CouponPrice", couponPrice.ToString(CultureInfo.InvariantCulture), cookieOptions);
                        return Ok(new { success = true, message = "Apply mã giảm giá thành công" });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding apply coupon cookie: {ex.Message}");
                        return Ok(new { success = false, message = "Mã giảm giá không tồn tại" });
                    }
                }
                else
                {
                    return Ok(new { success = false, message = "Mã giảm giá đã hết hạn hoặc hết số lượng" });
                }

            }
            else
            {
                // nếu coupon không tồn tại
                return Ok(new { success = false, message = "Coupon not existed" });
            }

            return Json(new { CouponTitle = couponTitle });
        }
        public IActionResult GetCartView()
        {
            return ViewComponent("Cart");

        }
    }
}
    