using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;
using TechSavvy.Repository.Components;

namespace TechSavvy.Controllers
{
    public class BrandController : Controller
    {
        public readonly DataContext _dataContext;
        public BrandController (DataContext context)
        {
            _dataContext = context;
        }

        [Route("brand/{slug}")]
        public async Task<IActionResult> Index(string Slug = "", string sort_by = "")
        {
            var brand = _dataContext.Brands.FirstOrDefault(b => b.Slug == Slug);
            if (brand == null) return RedirectToAction("Index", "Home");

            IQueryable<ProductModel> products = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Ratings)
                .Where(p => p.BrandId == brand.Id && p.Quantity > 0);

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

            var productList = await products.ToListAsync();
            var productWithRatings = productList.Select(p =>
            {
                var avgStar = p.Ratings.Any()
                    ? (int)Math.Ceiling(p.Ratings.Average(r => r.Star))
                    : 0;

                return new ProductWithRatingViewModel
                {
                    Product = p,
                    AverageStar = avgStar
                };
            }).ToList();

            return View(productWithRatings);
        }

    }
}
