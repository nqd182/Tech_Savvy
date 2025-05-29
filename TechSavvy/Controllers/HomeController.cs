using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Models.ViewModels;
using TechSavvy.Repository;
using TechSavvy.Repository.Components;

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
            .Include(p => p.Brand)
            .Include(p => p.Ratings)
            .OrderByDescending(p => p.Id)
            .Where(p => p.Quantity > 0 && !p.IsDeleted);

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
        List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        CartItemViewModel cartVM = new()
        {
            CartItems = cartItems,
            GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),

        };
        ViewBag.CartHome = cartItems;
        return View(productWithRatings);
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
        var exists = await _dataContext.Wishlists
        .AnyAsync(w => w.ProductId == Id && w.UserId == user.Id);

        if (exists)
        {
            return Ok(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích." });
        }
        var wishlistProduct = new WishlistModel
        {
            ProductId = Id,
            UserId = user.Id
        };

        _dataContext.Wishlists.Add(wishlistProduct);
        try
        {
            await _dataContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Thêm sản phẩm vào danh sách yêu thích thành công" });
        }
        catch (Exception)
        {
            return StatusCode(500, "An Error occurred while adding to wishlist table.");
        }
    }
    public async Task<IActionResult> Wishlist()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account"); 
        }
        var wishlist_product = await (from w in _dataContext.Wishlists
                                      join p in _dataContext.Products on w.ProductId equals p.Id
                                      where w.UserId == user.Id 
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
    //public async Task<IActionResult> Contact()
    //{
    //    var contact = await _dataContext.Contact.FirstAsync();
    //    return View(contact);
    //}
}
