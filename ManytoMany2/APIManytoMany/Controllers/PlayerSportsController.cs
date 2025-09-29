using Microsoft.AspNetCore.Mvc;
using APIManytoMany.Models;
using APIManytoMany.DTOs;
using APIManytoMany.Repositories.Interfaces;

namespace APIManytoMany.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerSportsController : ControllerBase
    {
        private readonly IPlayerSportRepository _playerSportRepo;

        public PlayerSportsController(IPlayerSportRepository playerSportRepo)
        {
            _playerSportRepo = playerSportRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _playerSportRepo.GetAllAsync();

            var dtoList = list.Select(ps => new PlayerSportDto
            {
                SportId = ps.SportId,
                SportName = ps.Sport.SportName,
                YearsPlayed = ps.YearsPlayed,
                Position = ps.Position
            }).ToList();

            return Ok(dtoList);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PlayerSport ps)
        {
            var added = await _playerSportRepo.AddAsync(ps);
            return Ok(added);
        }
    }
}
