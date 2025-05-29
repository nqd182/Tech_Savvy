using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Repository;

namespace TechSavvy.Repository.Components
{
    public class CategoriesViewComponent: ViewComponent
    {
        public readonly DataContext _dataContext;
        public CategoriesViewComponent(DataContext context)
        {
            _dataContext = context;
        }
        public IViewComponentResult Invoke(string viewName = null)
        {
            var categories = _dataContext.Categories
                .Where(c => !c.IsDeleted && c.Status != 0)
                .ToList();

            // Nếu có viewName, trả về view đó, ngược lại trả về Default
            if (!string.IsNullOrEmpty(viewName))
                return View(viewName, categories);

            return View(categories);
        }
    }
}
