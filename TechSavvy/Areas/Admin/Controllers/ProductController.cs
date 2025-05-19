using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TechSavvy.Models;
using TechSavvy.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechSavvy.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("Admin/Product")]

    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment; // interface có sẵn trong ASP.NET Core, cung cấp thông tin về môi trường hosting (máy chủ đang chạy ứng dụng).
        public ProductController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var products = await _dataContext.Products
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.Id)
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .ToListAsync();

            return View(products);
        }
        [HttpGet]
        [Route("LinhKien")]
        public async Task<IActionResult> LinhKien()
        {
            // Giả sử Category.Name hoặc Slug là "Linh kiện máy tính"
            var linhKienProducts = await _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.Category.Name == "Linh kiện máy tính" && !p.IsDeleted)
                .ToListAsync();

            return View("LinhKien", linhKienProducts); // Tái sử dụng view Index
        }
        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");

            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken] // yêu cầu token hợp lệ khi rq post
        public async Task<IActionResult> Create(ProductModel product)
        {
            var categories = _dataContext.Categories
                            .Where(c => c.Name != "Linh kiện máy tính")
                            .ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            if (ModelState.IsValid) // ktra trạng thái model
            {
                TempData["success"] = "Thêm sản phẩm thành công";
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có slug trong database");
                    return View(product);
                }
                else
                {
                    if (product.ImageUpload != null)
                    {
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                        string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadsDir, imageName);
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await product.ImageUpload.CopyToAsync(fs); // Ghi nội dung của file ảnh upload vào FileStream
                        fs.Close();
                        product.Image = imageName;
                    }
                }
                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
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
            return View(product);
        }

        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            return View(product);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, ProductModel product)
        {
            var productInDb = await _dataContext.Products.FindAsync(Id);
            if (productInDb == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                productInDb.Name = product.Name;
                productInDb.Description = product.Description;
                productInDb.Slug = product.Name.Replace(" ", "-");
                productInDb.Price = product.Price;
                productInDb.CapitalPrice = product.CapitalPrice;
                productInDb.CategoryId = product.CategoryId;
                productInDb.BrandId = product.BrandId;

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }
                    productInDb.Image = imageName;
                }
               
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công";
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
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            //if (!string.Equals(product.Image, "test.jpg"))
            //{
            //    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
            //    string oldfileImage= Path.Combine(uploadsDir, product.Image);
            //    if (System.IO.File.Exists(oldfileImage))
            //    {
            //        System.IO.File.Delete(oldfileImage);
            //    }
            //}
            //_dataContext.Products.Remove(product);

            product.IsDeleted = true; // Xóa mềm
            _dataContext.Products.Update(product);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Đã xóa sản phẩm (mềm) thành công";
            return RedirectToAction("Index");
        }
        // Tạo số lượng sản phẩm
        [Route("AddQuantity/{id}")]
        [HttpGet]
        public async Task<IActionResult> AddQuantity(int Id)
        {
            var productbyquantity = await _dataContext.ProductQuantities.Where(pq => pq.ProductId == Id).ToListAsync();
            ViewBag.ProductByQuantity = productbyquantity; 
            ViewBag.Id = Id;
            return View();
        }
        [HttpPost]
        [Route("UpdateMoreQuantity")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMoreQuantity(ProductQuantityModel productQuantityModel)
        {
            // Get the product to update
            var product = _dataContext.Products.Find(productQuantityModel.ProductId);

            if (product == null)
            {
                return NotFound();
            }
            product.Quantity = (product.Quantity ?? 0) + productQuantityModel.Quantity;
            productQuantityModel.ProductId = productQuantityModel.ProductId;
            productQuantityModel.DateCreated = DateTime.Now;
            _dataContext.Add(productQuantityModel);
            _dataContext.Update(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thêm số lượng sản phẩm thành công";
            return RedirectToAction("AddQuantity", "Product", new { Id = productQuantityModel.ProductId });
        }
        [Route("Trash")]
        public IActionResult Trash()
        {
            var deletedProducts = _dataContext.Products
                .Where(p => p.IsDeleted)
                .ToList();

            return View(deletedProducts);
        }
        [Route("Restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null || !product.IsDeleted)
            {
                return NotFound();
            }

            product.IsDeleted = false;
            _dataContext.Products.Update(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Sản phẩm đã được khôi phục";
            return RedirectToAction("Trash");
        }
        // Xóa vĩnh viễn 
        [Route("DeletePermanent/{id}")]

        public async Task<IActionResult> DeletePermanent(int id)
        {
            var brand = await _dataContext.Products.FindAsync(id);
            if (brand == null) return NotFound();

            _dataContext.Products.Remove(brand);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Đã xóa vĩnh viễn sản phẩm";
            return RedirectToAction("Trash");
        }


    }
}
