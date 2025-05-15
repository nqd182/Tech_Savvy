using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Migrations;
using TechSavvy.Repository;
using TechSavvy.Models;
namespace TechSavvy.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("Admin/Shipping")]
    public class ShippingController : Controller
    {
        private readonly DataContext _dataContext;

        public ShippingController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var shippingList = await _dataContext.Shippings.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Shippings = shippingList;
            return View();
        }
    
        [HttpPost]
        [Route("StoreShipping")]
        public async Task<IActionResult> StoreShipping(ShippingModel shippingModel, string phuong, string quan, string tinh, decimal price)
        {

            shippingModel.City = tinh;
            shippingModel.District = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;

            try
            {

                var existingShipping = await _dataContext.Shippings
                    .AnyAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);
                //kiem tra co ton tai du lieu chua
                if (existingShipping)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp." });
                }
                _dataContext.Shippings.Add(shippingModel);
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm shipping thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while adding shipping.");
            }
        }
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shipping = await _dataContext.Shippings.FindAsync(id);
            if (shipping == null)
            {
                return NotFound();
            }
            shipping.IsDeleted = true; // Xóa mềm
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa shipping thành công";
            return RedirectToAction("Index");
        }
        [Route("Trash")]
        public IActionResult Trash()
        {
            var deletedShipping = _dataContext.Shippings
                .Where(p => p.IsDeleted)
                .ToList();

            return View(deletedShipping);
        }
        [Route("Restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var shipping = await _dataContext.Shippings.FindAsync(id);
            if (shipping == null || !shipping.IsDeleted)
            {
                return NotFound();
            }

            shipping.IsDeleted = false;
            _dataContext.Shippings.Update(shipping);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Shipping đã được khôi phục";
            return RedirectToAction("Trash");
        }
        [Route("DeletePermanent/{id}")]
        public async Task<IActionResult> DeletePermanent(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Đã xóa vĩnh viễn danh mục";
            return RedirectToAction("Trash");
        }
    }
}
