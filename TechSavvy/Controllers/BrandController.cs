using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy.Controllers
{
    public class BrandController : Controller
    {
        public readonly DataContext _dataContext;
        public BrandController (DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "", string sort_by = "")
        {
            var brand = _dataContext.Brands.FirstOrDefault(b => b.Slug == Slug);
            if (brand == null) return RedirectToAction("Index", "Home");

            IQueryable<ProductModel> products = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.BrandId == brand.Id);

            switch (sort_by)
            {
                case "price_increase":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_decrease":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "price_newest":
                    products = products.OrderByDescending(p => p.Id);
                    break;
                case "price_oldest":
                    products = products.OrderBy(p => p.Id);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            return View(await products.ToListAsync());
        }

    }
}
