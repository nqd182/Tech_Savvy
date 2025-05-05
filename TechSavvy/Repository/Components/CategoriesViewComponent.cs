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
        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Categories.ToListAsync()); 
    }
}
