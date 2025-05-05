using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;

namespace TechSavvy.Repository
{
    public class DataContext: IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }

        public DbSet<BrandModel> Brands { get; set; } //DbSet để ánh xạ model vs csdl
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<RatingModel> Ratings { get; set; }
        public DbSet<WishlistModel> Wishlists { get; set; }
        public DbSet<ProductQuantityModel> ProductQuantities { get; set; }
        public DbSet<ShippingModel> Shippings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

    }

}
