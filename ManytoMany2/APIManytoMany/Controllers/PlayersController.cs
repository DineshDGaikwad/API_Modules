using Microsoft.AspNetCore.Mvc;
using APIManytoMany.Models;
using APIManytoMany.DTOs;
using APIManytoMany.Repositories.Interfaces;

namespace APIManytoMany.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepo;
        private readonly IPlayerSportRepository _playerSportRepo;

        public PlayersController(IPlayerRepository playerRepo, IPlayerSportRepository playerSportRepo)
        {
            _playerRepo = playerRepo;
            _playerSportRepo = playerSportRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var players = await _playerRepo.GetAllAsync();

            var playerDtos = players.Select(player => new PlayerDto
            {
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                MobileNo = player.MobileNo,
                Address = player.Address,
                Sports = player.PlayerSports.Select(ps => new PlayerSportDto
                {
                    SportId = ps.SportId,
                    SportName = ps.Sport.SportName,
                    YearsPlayed = ps.YearsPlayed,
                    Position = ps.Position
                }).ToList()
            }).ToList();

            return Ok(playerDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var player = await _playerRepo.GetByIdAsync(id);
            if (player == null) return NotFound();

            var playerDto = new PlayerDto
            {
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                MobileNo = player.MobileNo,
                Address = player.Address,
                Sports = player.PlayerSports.Select(ps => new PlayerSportDto
                {
                    SportId = ps.SportId,
                    SportName = ps.Sport.SportName,
                    YearsPlayed = ps.YearsPlayed,
                    Position = ps.Position
                }).ToList()
            };

            return Ok(playerDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Player player)
        {
            var newPlayer = await _playerRepo.AddAsync(player);
            return CreatedAtAction(nameof(GetById), new { id = newPlayer.PlayerId }, newPlayer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Player player)
        {
            if (id != player.PlayerId) return BadRequest();
            await _playerRepo.UpdateAsync(player);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _playerRepo.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{playerId}/assign-sport/{sportId}")]
        public async Task<IActionResult> AssignSport(int playerId, int sportId, [FromBody] PlayerSport details)
        {
            details.PlayerId = playerId;
            details.SportId = sportId;

            var assigned = await _playerSportRepo.AddAsync(details);
            return Ok(assigned);
        }
    }
}
