using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
    [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuthorDto dto)
        {
            return CreatedAtAction(nameof(GetById), new { id = 1 }, null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorDto dto)
        {
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            return NoContent();
        }
    }
}

