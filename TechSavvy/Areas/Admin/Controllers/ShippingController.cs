using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Migrations;
using TechSavvy.Repository;
using TechSavvy.Models;
namespace TechSavvy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Route("Admin/Shipping")]
    public class ShippingController : Controller
    {
        private readonly DataContext _dataContext;

        public ShippingController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var shippingList = await _dataContext.Shippings.ToListAsync();
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
    }
}
