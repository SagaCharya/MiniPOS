using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestroApp.Core.Entities;
using RestroApp.Infrastructure.Data;
using System.Linq;

namespace RestroApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        return await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<Order>>> GetPendingOrders()
    {
        // SELF-HEALING: Remove any orders with 0 items that might have been left behind
        var ghostOrders = await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.Status == OrderStatus.Pending && (o.OrderItems == null || !o.OrderItems.Any()))
            .ToListAsync();

        if (ghostOrders.Any())
        {
            _context.Orders.RemoveRange(ghostOrders);
            await _context.SaveChangesAsync();
        }

        return await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Status != OrderStatus.Paid && o.Status != OrderStatus.Cancelled)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();
        return order;
    }

    [HttpPut("{id}/billing")]
    public async Task<IActionResult> UpdateBilling(int id, [FromBody] BillingUpdateRequest request)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.DiscountPercentage = request.DiscountPercentage;
        order.TaxPercentage = request.TaxPercentage;
        order.TotalAmount = request.TotalAmount;
        order.PaymentMethod = request.PaymentMethod;
        
        if (request.Paid)
        {
            order.Status = OrderStatus.Paid;
            // Also free up the table if it's currently occupied
            if (order.TableId.HasValue)
            {
                var table = await _context.Tables.FindAsync(order.TableId.Value);
                if (table != null) table.IsOccupied = false;
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Order>> PostOrder([FromBody] OrderCreateRequest request)
    {
        try
        {
            var order = new Order
            {
                TableId = request.TableId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                DiscountPercentage = 0,
                TaxPercentage = 0,
                TotalAmount = request.OrderItems.Sum(i => i.Quantity * i.UnitPrice),
                OrderItems = request.OrderItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SpecialInstructions = i.SpecialInstructions
                }).ToList()
            };

            _context.Orders.Add(order);

            // Mark table as occupied
            if (request.TableId.HasValue)
            {
                var table = await _context.Tables.FindAsync(request.TableId.Value);
                if (table != null) table.IsOccupied = true;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncOrder([FromBody] OrderCreateRequest request)
    {
        try
        {
            if (!request.TableId.HasValue) return BadRequest("Table ID is required.");

            // Find existing pending order for this table
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.TableId == request.TableId && o.Status != OrderStatus.Paid && o.Status != OrderStatus.Cancelled);

            // CASE 1: No items in request - skip sync (don't release table)
            if (request.OrderItems == null || request.OrderItems.Count == 0)
            {
                return Ok(new { message = "Empty sync skipped" });
            }

            // CASE 2: Items present - Sync
            if (order == null)
            {
                order = new Order
                {
                    TableId = request.TableId,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    DiscountPercentage = 0,
                    TaxPercentage = 0,
                    OrderItems = new List<OrderItem>()
                };
                _context.Orders.Add(order);
            }

            // Ensure table is occupied
            if (request.TableId.HasValue)
            {
                var table = await _context.Tables.FindAsync(request.TableId.Value);
                if (table != null && !table.IsOccupied)
                {
                    table.IsOccupied = true;
                }
            }

            // If items are added to an order that was already Ready/Served,
            // push it back to Preparing so the kitchen see it again.
            if (order.Status == OrderStatus.Ready || order.Status == OrderStatus.Served)
            {
                order.Status = OrderStatus.Preparing;
            }

            // Update items
            if (order.OrderItems != null)
            {
                _context.OrderItems.RemoveRange(order.OrderItems);
            }
            
            order.OrderItems = request.OrderItems.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                SpecialInstructions = i.SpecialInstructions
            }).ToList();

            order.TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);

            await _context.SaveChangesAsync();

            return Ok(new { orderId = order.Id, totalAmount = order.TotalAmount });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/items")]
    public async Task<IActionResult> UpdateOrderItems(int id, [FromBody] List<OrderItemRequest> items)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        // Update items
        _context.OrderItems.RemoveRange(order.OrderItems);
        
        order.OrderItems = items.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            SpecialInstructions = i.SpecialInstructions
        }).ToList();

        // Recalculate total based on items and current discounts/taxes
        var subtotal = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);
        var discountAmount = (subtotal * order.DiscountPercentage) / 100;
        var taxableAmount = subtotal - discountAmount;
        var taxAmount = (taxableAmount * order.TaxPercentage) / 100;
        order.TotalAmount = subtotal - discountAmount + taxAmount;

        await _context.SaveChangesAsync();

        var updatedOrder = await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        return Ok(updatedOrder);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateRequest request)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = request.Status;
        
        // If paid, ensure table is free
        if (order.Status == OrderStatus.Paid && order.TableId.HasValue)
        {
            var table = await _context.Tables.FindAsync(order.TableId.Value);
            if (table != null) table.IsOccupied = false;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    public class StatusUpdateRequest
    {
        public OrderStatus Status { get; set; }
    }

    public class OrderCreateRequest
    {
        public int? TableId { get; set; }
        public List<OrderItemRequest> OrderItems { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? SpecialInstructions { get; set; }
    }

    public class BillingUpdateRequest
    {
        public decimal DiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Paid { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}
