using APIJobPortal.DTOs.Job;
using APIJobPortal.Interfaces.Services;
using APIJobPortal.Models;
using APIJobPortal.Interfaces.Repositories;

namespace APIJobPortal.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepo _jobRepo;

        public JobService(IJobRepo jobRepo)
        {
            _jobRepo = jobRepo;
        }

        public async Task<GetJobDTO> CreateJobAsync(CreateJobDTO dto)
        {
            var job = new Job
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                Type = dto.Type,
                Salary = dto.Salary,
                PostedDate = DateTime.UtcNow,   
                ExpiryDate = dto.ExpiryDate ?? DateTime.UtcNow.AddMonths(1), 
                RequiredExperience = dto.RequiredExperience
            };

            await _jobRepo.AddAsync(job);

            return new GetJobDTO
            {
                JobId = job.JobId,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                Type = job.Type,
                Salary = job.Salary,
                PostedDate = job.PostedDate,
                ExpiryDate = job.ExpiryDate,   
                RequiredExperience = job.RequiredExperience
            };
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            return await _jobRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<GetJobDTO>> GetAllJobsAsync()
        {
            var jobs = await _jobRepo.GetAllAsync();

            return jobs.Select(j => new GetJobDTO
            {
                JobId = j.JobId,
                Title = j.Title,
                Description = j.Description,
                Location = j.Location,
                Type = j.Type,
                Salary = j.Salary,
                PostedDate = j.PostedDate,
                ExpiryDate = j.ExpiryDate,
                RequiredExperience = j.RequiredExperience
            });
        }

        public async Task<GetJobDTO?> GetJobByIdAsync(int id)
        {
            var job = await _jobRepo.GetByIdAsync(id);
            if (job == null) return null;

            return new GetJobDTO
            {
                JobId = job.JobId,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                Type = job.Type,
                Salary = job.Salary,
                PostedDate = job.PostedDate,
                ExpiryDate = job.ExpiryDate,
                RequiredExperience = job.RequiredExperience
            };
        }
    }
}
