using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(string Slug = "")
        {
            CategoryModel category = _dataContext.Categories.FirstOrDefault(c => c.Slug == Slug);
            if (category == null) return RedirectToAction("Index");
            var productsByCategory = _dataContext.Products.Where(p => p.CategoryId == category.Id);
            return View(await productsByCategory.OrderByDescending(p => p.Id).ToListAsync());
        }
   

    }
}
