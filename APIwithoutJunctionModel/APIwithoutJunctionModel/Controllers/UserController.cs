using APIwithoutJunctionModel.DTOs;
using APIwithoutJunctionModel.Interfaces;
using APIwithoutJunctionModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIwithoutJunctionModel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] CreateUserDTO dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var createdUser = await _userService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById), 
                new { id = createdUser.userId }, 
                createdUser
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUserDTO dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var updated = await _userService.UpdateAsync(id, dto);
            if (!updated) 
                return NotFound(new { message = $"User with ID {id} not found." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) 
                return NotFound(new { message = $"User with ID {id} not found." });

            return NoContent();
        }
    }
}
