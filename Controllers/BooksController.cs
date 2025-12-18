using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
        => Ok(new { message = "Return all books (TODO)" });

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
        => Ok(new { message = $"Return book {id} (TODO)" });

    [HttpPost]
    public IActionResult Create([FromBody] object body)
        => CreatedAtAction(nameof(GetById), new { id = 1 }, new { message = "Created book (TODO)" });

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] object body)
        => NoContent();

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
        => NoContent();
}