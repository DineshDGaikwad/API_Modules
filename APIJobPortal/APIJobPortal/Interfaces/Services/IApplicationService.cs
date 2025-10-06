using APIJobPortal.DTOs.Application;

namespace APIJobPortal.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<GetApplicationDTO> GetApplicationByIdAsync(int id);
        Task<IEnumerable<GetApplicationDTO>> GetAllApplicationsAsync();
        Task<GetApplicationDTO> CreateApplicationAsync(CreateApplicationDTO dto);
        Task<bool> DeleteApplicationAsync(int id);
    }
}
