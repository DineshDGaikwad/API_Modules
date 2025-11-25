using APIPropertyRegistry.DTOs;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileResponseDto?> GetProfileAsync(int userId);
        Task<UserProfileResponseDto> UpdateProfileAsync(int userId, UserProfileUpdateDto dto);
    }
}
