using APIManytoMany.Infrastructure;
using APIManytoMany.Domain;
using Microsoft.AspNetCore.Mvc;

namespace APIManytoMany.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            return Ok(await _service.GetAllUser());
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            await _service.AddUsr(user);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, User user)
        {
            var updatedUser = await _service.Update(id, user);
            if (updatedUser == null)
                return NotFound($"User with id {id} not found.");

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteUsr(id);
            if (!deleted)
                return NotFound($"User with id {id} not found.");

            return NoContent(); 
        }

    }
}
