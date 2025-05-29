using Microsoft.AspNetCore.Mvc;
using TechSavvy.Models; 
using System.Collections.Generic;
using System.Linq;
using TechSavvy.Models;
using TechSavvy.Repository;

public class CartViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        return View(cart);
    }
}
