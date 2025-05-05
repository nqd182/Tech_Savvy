using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy.Controllers
{
    public class BrandController : Controller
    {
        public readonly DataContext _datacontext;
        public BrandController (DataContext context)
        {
            _datacontext = context;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            BrandModel brand = _datacontext.Brands.FirstOrDefault(b => b.Slug == Slug);
            if (brand == null) RedirectToAction("Index");
            var productsByBrand = _datacontext.Products.Where(p => p.BrandId == brand.Id);

            return View(await productsByBrand.OrderByDescending(p=>p.Id).ToListAsync());
        }
    }
}
