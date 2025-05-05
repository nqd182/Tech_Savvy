using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;

namespace TechSavvy.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate(); // đảm bảo rằng db đã update migration ms nhất
            if (!_context.Products.Any()) // kiểm tra xem đã tồn tại dữ liệu nào trong bảng chưa
            {
                CategoryModel pcgaming = new CategoryModel { Name = "PC Gaming", Slug = "PC Gaming", Description = "Apple is largest brand in the word", Status = 1 };
                CategoryModel pcworkstation = new CategoryModel { Name = "PC Workstation", Slug = "PC Workstation", Description = "Msi is famoust brand in the word", Status = 1 };
                BrandModel asus = new BrandModel { Name = "Asus", Slug = "Asus", Description = "Apple is largest brand in the word", Status = 1 };
                BrandModel msi = new BrandModel { Name = "Msi", Slug = "Asus", Description = "Msi is famoust brand in the word", Status = 1 };

                _context.Products.AddRange(
                    new ProductModel { Name = "PC HIỆU SUẤT GAMING CAO RTX 4060 - I5 12400F", Slug = "PC RTX 4060", Description = "Apple is largest brand in the word", Price = 1000, Category = pcgaming, Brand = asus, Image = "1.jpg" },
                    new ProductModel { Name = "PC 4K GAMING RTX 4070 12400F", Slug = "PC 4K GAMING", Description = "Apple is largest brand in the word", Price = 1000, Category = pcworkstation, Brand = msi, Image = "2.jpg" }

                );
                _context.SaveChanges();
            }

            
        }
    }
}
