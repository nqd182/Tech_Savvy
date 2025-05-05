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
        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Brands.ToListAsync());
    }
}
