using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _dataContext;
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUserModel> _userManager;

    public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager)
    {
        _logger = logger;
        _dataContext = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string sort_by = "")
    {
        IQueryable<ProductModel> products = _dataContext.Products
            .Include(p => p.Category)
            .Include(p => p.Brand);

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
                products = products.OrderBy(p => p.Name); // mặc định
                break;
        }

        return View(await products.ToListAsync());
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int statuscode)
    {
        if(statuscode == 404)
        {
            return View("Notfound");
        }
        else
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        }
    }
    public async Task<IActionResult> AddWishlist(int Id, WishlistModel wishlist)
    {
        var user = await _userManager.GetUserAsync(User);

        var wishlistProduct = new WishlistModel
        {
            ProductId = Id,
            UserId = user.Id
        };

        _dataContext.Wishlists.Add(wishlistProduct);
        try
        {
            await _dataContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Add to wishlisht Successfully" });
        }
        catch (Exception)
        {
            return StatusCode(500, "An Error occurred while adding to wishlist table.");
        }
    }
    public async Task<IActionResult> Wishlist()
    {
        var wishlist_product = await (from w in _dataContext.Wishlists
                                      join p in _dataContext.Products on w.ProductId equals p.Id
                                      select new { Product = p, Wishlists = w })
                           .ToListAsync();

        return View(wishlist_product);
    }
    public async Task<IActionResult> DeleteWishlist(int Id)
    {
        WishlistModel wishlist = await _dataContext.Wishlists.FindAsync(Id);

        _dataContext.Wishlists.Remove(wishlist);

        await _dataContext.SaveChangesAsync();
        TempData["success"] = "Xóa sản phẩm yêu thích thành công";
        return RedirectToAction("Wishlist", "Home");
    }
}
