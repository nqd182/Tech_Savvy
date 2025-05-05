using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connection Db 
            builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectDb")));
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Add cache to store session 
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(30);
                option.Cookie.IsEssential = true; // cookie của session là bắt buộc
            });

            builder.Services.AddIdentity<AppUserModel, IdentityRole>()
            .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders(); // LoginPath mặc định sAccount/Login

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;

                // User settings.
   
                options.User.RequireUniqueEmail = true;
            });
            var app = builder.Build();



            app.UseStatusCodePagesWithRedirects("/Home/Error/?statuscode={0}");

            app.UseSession();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapControllerRoute(
                name: "category",
                pattern: "/category/{Slug?}",
                defaults : new {controller="Category", action = "Index"});
            app.MapControllerRoute(
                name: "brand",
                pattern: "/brand/{Slug?}",
                defaults: new { controller = "Brand", action = "Index" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            //Seeding Data
            ////Seeding Data
            //var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>(); //Lấy DataContext từ Dependency Injection (DI) container
            //SeedData.SeedingData(context);
            app.Run();
        }
    }
}
