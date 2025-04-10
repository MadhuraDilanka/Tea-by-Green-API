using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaByGreen.Models;
using TeaByGreen;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // POST: /api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] User newUser)
    {
        // Optional: check if email already exists
        var exists = await _context.Users.AnyAsync(u => u.Email == newUser.Email);
        if (exists)
        {
            return BadRequest("Email already in use.");
        }

        // Optional: Hash the password here before saving
        newUser.PasswordHash = newUser.PasswordHash; // Replace with hash if needed

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, newUser);
    }
}
