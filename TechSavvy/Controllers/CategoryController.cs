using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;
using TechSavvy.Repository.Components;

namespace TechSavvy.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("category/{slug}")]
        public async Task<IActionResult> Index(string Slug = "", string sort_by="")
        {
            CategoryModel category = _dataContext.Categories.FirstOrDefault(c => c.Slug == Slug);
            if (category == null) return RedirectToAction("Index", "Home");
            IQueryable<ProductModel> productsByCategory = _dataContext.Products
                .Where(p => p.CategoryId == category.Id &&  p.Quantity > 0)
                .Include(p => p.Brand)
                .Include(p => p.Ratings);
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
            var productList = await productsByCategory.ToListAsync();
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
