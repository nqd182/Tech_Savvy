﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("Admin/Order")]

    public class OrderController : Controller
    {

        private readonly DataContext _dataContext;
        public OrderController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
        }
        [HttpGet]
        [Route("ViewOrder/{ordercode}")]
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var detailsOrder = await _dataContext.OrderDetails.Include(od => od.Product).Where(od=>od.OrderCode == ordercode).ToListAsync();
            // lấy giá ship
            var shippingCost = _dataContext.Orders.Where(o => o.OrderCode == ordercode).First();
            ViewBag.ShippingCost = shippingCost.ShippingCost;
            return View(detailsOrder);
        }
        [HttpPost]
        [Route("UpdateOrder/{ordercode}")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if(order == null)
            {
                return NotFound();
            }
            order.Status = status;
            if(status == 0)
            {
                var DetailsOrder = await _dataContext.OrderDetails.Where(od => od.OrderCode == ordercode)
                    .Select(od => new { 
                        od.Quantity, 
                        od.Product.Price,
                        od.Product.CapitalPrice,
                    })
                    .ToListAsync();
                var statiscalModel = await _dataContext.Statisticals.FirstOrDefaultAsync(s => s.DateCreated.Date == order.CreatedDate.Date);
                if (statiscalModel != null) {
                    foreach (var item in DetailsOrder)
                    {
                        statiscalModel.Quantity += 1;
                        statiscalModel.Sold += item.Quantity;
                        statiscalModel.Revenue += item.Quantity * item.Price;
                        statiscalModel.Profit += item.Quantity * (item.Price - item.CapitalPrice);
                    }
                    _dataContext.Update(statiscalModel);
                }
                else
                {
                    int new_quantity = 0;
                    int new_sold = 0;
                    decimal new_profit = 0;
                    foreach (var item in DetailsOrder)
                    {
                        new_quantity += 1;
                        new_sold += item.Quantity;
                        new_profit += item.Quantity * (item.Price - item.CapitalPrice);
                        statiscalModel = new StatisticalModel
                        {
                            Quantity = new_quantity,
                            Sold = new_sold,
                            Revenue = item.Quantity * item.Price,
                            Profit = new_profit,
                            DateCreated = order.CreatedDate,
                        };
                    }
                    _dataContext.Add(statiscalModel);
                }
            }
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order status updated successfully" });
            }catch(Exception ex)
            {
                return StatusCode(500, "An Error occured while updating the order status");
            }
            return View();
        }
    }
}
