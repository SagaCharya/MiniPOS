using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestroApp.Core.Entities;
using RestroApp.Infrastructure.Data;

namespace RestroApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        return await _context.Categories.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return category;
    }

    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(Category category)
    {
        try
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving category: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            return BadRequest(ex.Message);
        }
    }
}
