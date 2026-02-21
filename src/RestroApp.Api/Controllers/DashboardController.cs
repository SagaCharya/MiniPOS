using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestroApp.Core.Entities;
using RestroApp.Infrastructure.Data;

namespace RestroApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<ActionResult> GetStats()
    {
        var today = DateTime.UtcNow.Date;
        
        var completedOrders = await _context.Orders
            .Where(o => o.Status == OrderStatus.Paid || o.Status == OrderStatus.Served)
            .ToListAsync();

        var totalRevenue = completedOrders.Sum(o => o.TotalAmount);
        var totalOrders = completedOrders.Count;
        
        var todaysOrders = completedOrders
            .Where(o => o.OrderDate.Date == today)
            .ToList();
            
        var todaysSales = todaysOrders.Sum(o => o.TotalAmount);
        var todaysCount = todaysOrders.Count;
        
        var averageOrder = totalOrders > 0 ? totalRevenue / totalOrders : 0;

        var salesByMethod = completedOrders
            .Where(o => o.PaymentMethod.HasValue)
            .GroupBy(o => o.PaymentMethod)
            .Select(g => new
            {
                Method = g.Key.ToString(),
                Amount = g.Sum(o => o.TotalAmount),
                Count = g.Count()
            }).ToList();

        return Ok(new
        {
            totalRevenue,
            totalOrders,
            todaysSales,
            todaysCount,
            averageOrder,
            salesByMethod
        });
    }

    [HttpGet("top-items")]
    public async Task<ActionResult> GetTopItems()
    {
        var topItems = await _context.OrderItems
            .Include(oi => oi.Product)
            .GroupBy(oi => new { oi.ProductId, oi.Product!.Name })
            .Select(g => new
            {
                Name = g.Key.Name,
                Sold = g.Sum(oi => oi.Quantity),
                Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(g => g.Sold)
            .Take(5)
            .ToListAsync();

        return Ok(topItems);
    }

    [HttpGet("recent-orders")]
    public async Task<ActionResult> GetRecentOrders()
    {
        var recentOrders = await _context.Orders
            .Include(o => o.Table)
            .OrderByDescending(o => o.OrderDate)
            .Take(10)
            .Select(o => new {
                o.Id,
                TableName = o.Table != null ? o.Table.TableNumber : "Takeaway",
                o.OrderDate,
                o.TotalAmount,
                Status = o.Status.ToString(),
                ItemCount = o.OrderItems.Count
            })
            .ToListAsync();

        return Ok(recentOrders);
    }
}
