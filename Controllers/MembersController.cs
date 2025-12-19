using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
        => Ok(new { message = "Return all members (TODO)" });

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
        => Ok(new { message = $"Return member {id} (TODO)" });

    [HttpPost]
    public IActionResult Create([FromBody] object body)
        => CreatedAtAction(nameof(GetById), new { id = 1 }, new { message = "Created member (TODO)" });

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] object body)
        => NoContent();

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
        => NoContent();
}