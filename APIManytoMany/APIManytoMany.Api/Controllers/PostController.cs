// using APIManytoMany
using APIManytoMany.Domain;
using Microsoft.AspNetCore.Mvc;

namespace APIManytoMany.Api.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly Infra.

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAll()
        {
            return Ok(await _service.GetAllPost());
        }

        [HttpPost]
        public async Task<ActionResult<Post>> AddUser(Post post)
        {
            await _service.AddUsr(post);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Post>> Update(int id, Post post)
        {
            var updatedPost = await _service.Update(id, post);
            if (updatedPost == null)
                return NotFound($"Post with id {id} not found.");

            return Ok(updatedPost);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteUsr(id);
            if (!deleted)
                return NotFound($"Post with id {id} not found.");

            return NoContent(); 
        }

    }
}
