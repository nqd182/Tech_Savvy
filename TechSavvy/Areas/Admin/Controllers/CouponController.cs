using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("Admin/Coupon")]
    public class CouponController : Controller
    {
            private readonly DataContext _dataContext;
            public CouponController(DataContext context)
            {
                _dataContext = context;
            }
            [Route("Index")]
            public async Task<IActionResult> Index()
            {
                var coupon_list = await _dataContext.Coupons.Where(b => !b.IsDeleted).ToListAsync();
                ViewBag.Coupons = coupon_list;
                return View();
            }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponModel coupon)
        {


            if (ModelState.IsValid)
            {

                _dataContext.Add(coupon);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm coupon thành công";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var coupon = _dataContext.Coupons.Find(id); // Giả sử bạn dùng EF
            if (coupon == null) return NotFound();

            return View(coupon);
        }

        [HttpPost]
        public IActionResult Edit(CouponModel model)
        {
            if (ModelState.IsValid)
            {
                _dataContext.Coupons.Update(model);
                _dataContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _dataContext.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();

            coupon.IsDeleted = true; // Đánh dấu là đã xóa
            _dataContext.Coupons.Update(coupon);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Đã xóa coupon thành công";
            return RedirectToAction("Index");
        }
        [Route("Trash")]
        public async Task<IActionResult> Trash()
        {
            var deletedCoupons = await _dataContext.Coupons
                .Where(c => c.IsDeleted)
                .ToListAsync();

            return View(deletedCoupons);
        }
        [Route("Restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var coupon = await _dataContext.Coupons.FindAsync(id);
            if (coupon == null || !coupon.IsDeleted) return NotFound();

            coupon.IsDeleted = false; // Bỏ đánh dấu xóa
            _dataContext.Coupons.Update(coupon);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Coupon đã được khôi phục";
            return RedirectToAction("Trash");
        }
        [Route("DeletePermanent/{id}")]
        public async Task<IActionResult> DeletePermanent(int id)
        {
            var coupon = await _dataContext.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();

            _dataContext.Coupons.Remove(coupon); // Xóa khỏi database
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Đã xóa vĩnh viễn coupon";
            return RedirectToAction("Trash");
        }
    }
}
