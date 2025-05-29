using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;
using TechSavvy.Models;
using TechSavvy.Models.ViewModels;
using TechSavvy.Repository;
using TechSavvy.Repository.Components;

namespace TechSavvy.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("Product")]
        public async Task<IActionResult> Index(string sort_by = "")
        {
            IQueryable<ProductModel> products = _dataContext.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Ratings)
            .Where(p => p.Quantity > 0 && !p.IsDeleted && p.Category.Name != "Linh kiện máy tính");

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
        [Route("LinhKien")]
        public async Task<IActionResult> LinhKien(string sort_by = "")
        {
            IQueryable<ProductModel> products = _dataContext.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Ratings)
            .Where(p => p.Quantity > 0 && !p.IsDeleted && p.Category.Name == "Linh kiện máy tính");

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
        public async Task<IActionResult> Details(int Id)
        {

            ProductModel productById = _dataContext.Products.Include(p => p.Ratings).FirstOrDefault(p => p.Id == Id);
            if (productById == null) return RedirectToAction("Index");
            var relatedProduct = await _dataContext.Products
              .Include(p => p.Category)
              .Include(p => p.Brand)
              .Where(p => p.CategoryId == productById.CategoryId && p.Id != productById.Id)
              .Take(4)
              .ToListAsync();
            // Lấy danh sách đánh giá cho sản phẩm
            var reviews = _dataContext.Ratings
                .Where(r => r.ProductId == Id)
                .OrderByDescending(r => r.Id)
                .ToList();
            var viewModel = new ProductDetailsViewModel
            {
                ProductDetails = productById,
                Reviews = reviews
            };
            ViewBag.RelatedProducts = relatedProduct;
            return View(viewModel);
        }
        [Route("Search")]
        public async Task<IActionResult> Search(string searchItem, string sort_by = "")
        {
            if (string.IsNullOrEmpty(searchItem))
            {
                return View(new List<ProductWithRatingViewModel>());
            }
            IQueryable<ProductModel> productsBySearch =  _dataContext.Products.Where(p => p.Name.Contains(searchItem.ToLower()) || p.Description.Contains(searchItem) && p.Quantity > 0)
                                                                           .Include(p => p.Brand)
                                                                           .Include(p => p.Category)
                                                                           .Include(p => p.Ratings);
            ViewBag.Keyword = searchItem;
            var count = await productsBySearch.CountAsync();
            if (count > 0)
            {
                if (sort_by == "price_increase")
                {
                    productsBySearch = productsBySearch.OrderBy(p => p.Price);
                }
                else if (sort_by == "price_decrease")
                {
                    productsBySearch = productsBySearch.OrderByDescending(p => p.Price);
                }
                else if (sort_by == "price_newest")
                {
                    productsBySearch = productsBySearch.OrderByDescending(p => p.Id);
                }
                else if (sort_by == "price_oldest")
                {
                    productsBySearch = productsBySearch.OrderBy(p => p.Id);
                }
                else
                {
                    productsBySearch = productsBySearch.OrderBy(p => p.Price);
                }
            }
            var productList = await productsBySearch.ToListAsync();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentProduct(RatingModel rating)
        {
            if(ModelState.IsValid)

            {

                var ratingEntity = new RatingModel
                {
                    ProductId = rating.ProductId,
                    Name = rating.Name,
                    Email = rating.Email,
                    Comment = rating.Comment,
                    Star = rating.Star

                };

                _dataContext.Ratings.Add(ratingEntity);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thêm đánh giá thành công";

                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);

                return RedirectToAction("Detail", new { id = rating.ProductId });
            }

            return Redirect(Request.Headers["Referer"]);
        }
    }
}