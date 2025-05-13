using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("Admin/Category")]

    public class CategoryController : Controller

    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Categories.Where(c => !c.IsDeleted).OrderByDescending(p => p.Id).ToListAsync());
        }
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)     
            {
                TempData["success"] = "Thêm danh mục thành công";
                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã trong database");
                    return View(category);
                }
                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có 1 vài thứ đang bị lỗi";
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
            return View(category);
        }
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);

            return View(category);
        }
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, CategoryModel category)
        {
            var categoryInDb = await _dataContext.Categories.FindAsync(Id);
            if (categoryInDb == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                categoryInDb.Name = category.Name;
                categoryInDb.Description = category.Description;
                categoryInDb.Slug = category.Name.Replace(" ", "-");
                categoryInDb.Status = category.Status;
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có 1 vài thứ đang bị lỗi";
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
        }
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);
            //_dataContext.Categories.Remove(category);
            category.IsDeleted = true; // Xóa mềm
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Đã xóa danh mục thành công";
            return RedirectToAction("Index");
        }
        [Route("Trash")]
        public async Task<IActionResult> Trash()
        {
            var deletedCategories = await _dataContext.Categories
                .Where(c => c.IsDeleted)
                .ToListAsync();

            return View(deletedCategories);
        }

        [Route("Restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null || !category.IsDeleted) return NotFound();

            category.IsDeleted = false;
            _dataContext.Categories.Update(category);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Đã khôi phục danh mục";
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
