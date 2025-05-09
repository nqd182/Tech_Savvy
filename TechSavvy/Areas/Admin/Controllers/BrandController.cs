using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("Admin/Brand")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var brands = await _dataContext.Brands.OrderByDescending(b => b.Id).ToListAsync();
            return View(brands);
        }

        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slugExists = await _dataContext.Brands.AnyAsync(b => b.Slug == brand.Slug);
                if (slugExists)
                {
                    ModelState.AddModelError("", "Thương hiệu đã tồn tại trong database");
                    return View(brand);
                }

                _dataContext.Brands.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm thương hiệu thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Model có một vài lỗi";
            return View(brand);
        }

        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Admin/Brand/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandModel brand)
        {
            var brandInDb = await _dataContext.Brands.FindAsync(id);
            if (brandInDb == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                brandInDb.Name = brand.Name;
                brandInDb.Description = brand.Description;
                brandInDb.Slug = brand.Name.Replace(" ", "-");
                brandInDb.Status = brand.Status;

                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thương hiệu thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Model có một vài lỗi";
            return View(brand);
        }

        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa thương hiệu thành công";
            return RedirectToAction("Index");
        }
    }
}
