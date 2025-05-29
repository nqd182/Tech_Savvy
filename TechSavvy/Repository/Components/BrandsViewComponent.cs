using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Repository;

namespace TechSavvy.Repository.Components
{
    public class BrandsViewComponent : ViewComponent
    {
        public readonly DataContext _dataContext;
        public BrandsViewComponent(DataContext context)
        {
            _dataContext = context;
        }
        public IViewComponentResult Invoke(string viewName = null)
        {
            var brands = _dataContext.Brands
                .Where(c => !c.IsDeleted && c.Status != 0)
                .ToList();

            // Nếu có viewName, trả về view đó, ngược lại trả về Default
            if (!string.IsNullOrEmpty(viewName))
                return View(viewName, brands);

            return View(brands);
        }
    }
}
    