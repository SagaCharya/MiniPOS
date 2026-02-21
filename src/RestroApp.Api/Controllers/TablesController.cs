using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestroApp.Core.Entities;
using RestroApp.Infrastructure.Data;

namespace RestroApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TablesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiningTable>>> GetTables()
    {
        return await _context.Tables.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DiningTable>> GetTable(int id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return NotFound();
        return table;
    }

    [HttpPost]
    public async Task<ActionResult<DiningTable>> PostTable(DiningTable table)
    {
        try
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTable), new { id = table.Id }, table);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTable(int id, DiningTable table)
    {
        if (id != table.Id) return BadRequest();

        _context.Entry(table).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TableExists(id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTable(int id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return NotFound();

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TableExists(int id)
    {
        return _context.Tables.Any(e => e.Id == id);
    }
}
