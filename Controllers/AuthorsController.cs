using Library.Application.Dtos.Authors;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authors;

    public AuthorsController(IAuthorService authors)
    {
        _authors = authors;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuthorDto>>> GetAll(CancellationToken ct)
    {
        var items = await _authors.GetAllAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorDto>> GetById(int id, CancellationToken ct)
    {
        var item = await _authors.GetByIdAsync(id, ct);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorDto dto, CancellationToken ct)
    {
        var created = await _authors.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorDto dto, CancellationToken ct)
    {
        var updated = await _authors.UpdateAsync(id, dto, ct);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _authors.DeleteAsync(id, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }
}