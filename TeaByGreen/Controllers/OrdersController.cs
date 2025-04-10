using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaByGreen.Models;
using TeaByGreen;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    // POST: /api/orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        //{
        //"items": [
        //            {
        //    "productId": 1,
        //        "quantity": 2
        //                },
        //    {
        //    "productId": 2,
        //        "quantity": 1
        //    }
        //    ]
        //}

        order.OrderDate = DateTime.UtcNow;
        decimal total = 0;

        foreach (var item in order.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null || product.Stock < item.Quantity)
            {
                return BadRequest($"Product ID {item.ProductId} not found or not enough stock.");
            }

            product.Stock -= item.Quantity;
            item.Product = product;
            item.Order = order;
            item.OrderId = order.Id;
            total += product.Price * item.Quantity;
        }

        order.TotalAmount = total;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    // GET: /api/orders/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return order;
    }
}
