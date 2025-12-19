using Library.Application.Dtos.Books;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _books;

    public BooksController(IBookService books)
    {
        _books = books;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookDto>>> GetAll(CancellationToken ct)
    {
        var items = await _books.GetAllAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDto>> GetById(int id, CancellationToken ct)
    {
        var item = await _books.GetByIdAsync(id, ct);
        if (item is null) return NotFound();
        return Ok(item);
    }

    // Relationship endpoint
    [HttpGet("by-author/{authorId:int}")]
    public async Task<ActionResult<List<BookDto>>> GetByAuthorId(int authorId, CancellationToken ct)
    {
        var items = await _books.GetByAuthorIdAsync(authorId, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _books.CreateAsync(dto, ct);
            if (created is null) return BadRequest(new { message = "AuthorId does not exist." });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DbUpdateException)
        {
            // Typically ISBN unique constraint violation
            return Conflict(new { message = "A book with that ISBN already exists." });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto, CancellationToken ct)
    {
        try
        {
            var updated = await _books.UpdateAsync(id, dto, ct);
            if (!updated) return NotFound(); // could also be invalid AuthorId
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "A book with that ISBN already exists." });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _books.DeleteAsync(id, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }
}