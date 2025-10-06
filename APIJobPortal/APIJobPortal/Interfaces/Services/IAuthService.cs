using APIJobPortal.DTOs.Auth;

namespace APIJobPortal.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO?> LoginAsync(LoginDTO dto);
    }
}
