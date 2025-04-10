using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaByGreen.Models;

namespace TeaByGreen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/cart/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<Cart>> GetCart(int userId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return NotFound("Cart not found for this user.");

            return cart;
        }

        // POST: /api/cart/{userId}/add
        [HttpPost("{userId}/add")]
        public async Task<IActionResult> AddToCart(int userId, [FromBody] CartItem cartItem)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var product = await _context.Products.FindAsync(cartItem.ProductId);
            if (product == null) return NotFound("Product not found.");

            cartItem.Product = product;
            cart.Items.Add(cartItem);

            await _context.SaveChangesAsync();
            return Ok(cartItem);
        }

        // POST: /api/cart/{userId}/checkout
        [HttpPost("{userId}/checkout")]
        public async Task<IActionResult> Checkout(int userId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.Items.Count == 0)
                return BadRequest("Cart is empty or not found.");

            // Process order here
            var order = new Order { UserId = userId, OrderDate = DateTime.UtcNow, Items = new List<OrderItem>() };
            decimal total = 0;

            foreach (var cartItem in cart.Items)
            {
                var product = cartItem.Product;
                if (product.Stock < cartItem.Quantity)
                    return BadRequest($"Not enough stock for product {product.Name}.");

                product.Stock -= cartItem.Quantity;
                total += product.Price * cartItem.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    PriceAtOrderTime = product.Price
                };
                order.Items.Add(orderItem);
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            _context.Carts.Remove(cart); // Clear the cart after checkout

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCart), new { userId = userId }, order);
        }
    }

}
