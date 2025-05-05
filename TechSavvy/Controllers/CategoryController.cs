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

        public async Task<IActionResult> Index(string Slug = "", string sort_by="")
        {
            CategoryModel category = _dataContext.Categories.FirstOrDefault(c => c.Slug == Slug);
            if (category == null) return RedirectToAction("Index");
            IQueryable<ProductModel> productsByCategory = _dataContext.Products.Where(p => p.CategoryId == category.Id);
            var count = await productsByCategory.CountAsync();
            if (count > 0) 
            {
                if (sort_by == "price_increase")
                {
                    productsByCategory = productsByCategory.OrderBy(p => p.Price);
                }
                else if (sort_by == "price_decrease")
                {
                    productsByCategory = productsByCategory.OrderByDescending(p => p.Price);
                }
                else if (sort_by == "price_newest")
                {
                    productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
                }
                else if (sort_by == "price_oldest")
                {
                    productsByCategory = productsByCategory.OrderBy(p => p.Id);
                }
                else
                {
                    productsByCategory = productsByCategory.OrderBy(p => p.Price);
                }
            }
            return View(await productsByCategory.ToListAsync());
        }
   

    }
}
