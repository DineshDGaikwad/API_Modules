using APIJobPortal.DTOs.Job;

namespace APIJobPortal.Interfaces.Services
{
    public interface IJobService
    {
        Task<GetJobDTO> GetJobByIdAsync(int id);
        Task<IEnumerable<GetJobDTO>> GetAllJobsAsync();
        Task<GetJobDTO> CreateJobAsync(CreateJobDTO dto);
        Task<bool> DeleteJobAsync(int id);
    }
}
