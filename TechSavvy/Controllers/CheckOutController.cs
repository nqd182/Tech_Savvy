using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using TechSavvy.Models;
using TechSavvy.Models.ViewModels;
using TechSavvy.Repository;
using TechSavvy.Services.Momo;

namespace TechSavvy.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly DataContext _dataContext;
        private IMomoService _momoService;

        public CheckOutController(DataContext dataContext, IMomoService momoService)
        {
            _dataContext = dataContext;
            _momoService = momoService;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Nhận shipping giá từ cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            // Nhận coupon từ cookie
            var coupon_code = Request.Cookies["CouponTitle"];
            var couponPriceCookie = Request.Cookies["CouponPrice"];
            decimal couponPrice = 0;

            if (!string.IsNullOrEmpty(couponPriceCookie))
            {
                var couponPriceJson = couponPriceCookie;
                decimal.TryParse(couponPriceCookie, NumberStyles.Any, CultureInfo.InvariantCulture, out couponPrice);
            }

            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),
                ShippingCost = shippingPrice,
                CouponCode = coupon_code,
                Discount = couponPrice

            };
            return View(cartVM);

        }
        public async Task<IActionResult> Checkout(string OrderId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orderCode = Guid.NewGuid().ToString();
            var orderItem = new OrderModel
            {
                OrderCode = orderCode,
                UserName = userEmail,
                Status = 1,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            // 1. Nhận giá trị từ cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            var shippingAddress = Request.Cookies["ShippingAddress"];
            var couponCodeFull = Request.Cookies["CouponTitle"];
            var couponPriceCookie = Request.Cookies["CouponPrice"];

            decimal shippingPrice = 0;
            decimal couponDiscount = 0;

            if (!string.IsNullOrEmpty(shippingPriceCookie))
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceCookie);

            if (!string.IsNullOrEmpty(couponPriceCookie))
                couponDiscount = JsonConvert.DeserializeObject<decimal>(couponPriceCookie);

            orderItem.ShippingCost = shippingPrice;
            orderItem.CouponCode = couponCodeFull ?? "";
            orderItem.CouponPrice = couponDiscount;
            orderItem.ShippingAddress = shippingAddress;
            if(OrderId != null)
            {
                orderItem.PaymentMethod = "Momo: " + OrderId;
            }
            else
            {
                orderItem.PaymentMethod = "COD";
            }

            // 2. Giảm số lượng coupon nếu có mã hợp lệ
            string couponCode = null;
            if (!string.IsNullOrEmpty(couponCodeFull))
            {
                // Tách chuỗi "GIAM100K | Giảm giá 100k tổng sản phẩm" => "GIAM100K"
                couponCode = couponCodeFull.Split('|')[0].Trim();
            }
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _dataContext.Coupons
                    .FirstOrDefaultAsync(c => c.Name == couponCode && c.Quantity > 0);

                if (coupon != null)
                {
                    coupon.Quantity -= 1;
                    _dataContext.Coupons.Update(coupon);
                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = coupon.Name;
                }
                else
                {
                    TempData["warning"] = "Mã giảm giá không hợp lệ hoặc đã hết lượt sử dụng.";
                }
            }


            // 3. Lấy giỏ hàng từ session
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // 4. Tính tổng tiền sản phẩm
            decimal totalProductPrice = 0;
            foreach (var cart in cartItems)
            {
                totalProductPrice += cart.Price * cart.Quantity;
            }

            // 5. Tính tổng tiền cuối cùng
            orderItem.TotalPrice = totalProductPrice + shippingPrice - couponDiscount;

            // 6. Lưu đơn hàng
            _dataContext.Orders.Add(orderItem);
            await _dataContext.SaveChangesAsync();

            // 7. Lưu chi tiết đơn hàng và cập nhật tồn kho sản phẩm
            foreach (var cart in cartItems)
            {
                var orderDetail = new OrderDetails
                {
                    OrderCode = orderCode,
                    ProductId = cart.ProductId,
                    UnitPrice = cart.Price,
                    Quantity = cart.Quantity
                };

                var product = await _dataContext.Products.FirstAsync(p => p.Id == cart.ProductId);
                product.Quantity -= cart.Quantity;
                product.Sold = (product.Sold ?? 0) + cart.Quantity;

                _dataContext.OrderDetails.Add(orderDetail);
                _dataContext.Products.Update(product);
            }

            await _dataContext.SaveChangesAsync();

            // 8. Xóa session và cookie
            HttpContext.Session.Remove("Cart");
            Response.Cookies.Delete("ShippingPrice");
            Response.Cookies.Delete("CouponTitle");
            Response.Cookies.Delete("ShippingAddress");
            Response.Cookies.Delete("CouponPrice");
            TempData["success"] = "Đặt hàng thành công vui lòng chờ duyệt đơn hàng!";
            return RedirectToAction("History", "Account");
        }

       
        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(MomoInforModel model)
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;
            if (requestQuery["resultCode"] != "0")
            {
                var newMomoInsert = new MomoInforModel
                {
                    OrderId = requestQuery["orderId"],
                    FullName = User.FindFirstValue(ClaimTypes.Email),
                    Amount = decimal.Parse(requestQuery["Amount"]),
                    OrderInfo = requestQuery["orderInfo"],
                    DatePaid = DateTime.Now,
                };
                TempData["success"] = "Thanh toán thành công, vui lòng chờ duyệt đơn hàng";
                _dataContext.Add(newMomoInsert);
                await _dataContext.SaveChangesAsync();
                await Checkout(requestQuery["OrderId"]);
            }
            else
            {
                TempData["success"] = "Đã hủy giao dịch Momo";
                return RedirectToAction("Index", "Checkout");
            }
            return View(response);
        }
    }
}
