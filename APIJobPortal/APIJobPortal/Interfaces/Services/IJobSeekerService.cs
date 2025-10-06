using APIJobPortal.DTOs.JobSeeker;

namespace APIJobPortal.Interfaces.Services
{
    public interface IJobSeekerService
    {
        Task<GetJobSeekerDTO> GetJobSeekerByIdAsync(int id);
        Task<IEnumerable<GetJobSeekerDTO>> GetAllJobSeekersAsync();
        Task<GetJobSeekerDTO> CreateJobSeekerAsync(CreateJobSeekerDTO dto);
        Task<bool> DeleteJobSeekerAsync(int id);
    }
}
