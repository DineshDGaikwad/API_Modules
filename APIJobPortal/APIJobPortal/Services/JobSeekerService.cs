using APIJobPortal.DTOs.JobSeeker;
using APIJobPortal.Interfaces.Repositories;
using APIJobPortal.Interfaces.Services;
using APIJobPortal.Models;

namespace APIJobPortal.Services
{
    public class JobSeekerService : IJobSeekerService
    {
        private readonly IJobSeekerRepo _repo;

        public JobSeekerService(IJobSeekerRepo repo)
        {
            _repo = repo;
        }

        public async Task<GetJobSeekerDTO> CreateJobSeekerAsync(CreateJobSeekerDTO dto)
        {
            var jobSeeker = new JobSeeker
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Skills = dto.Skills,
                ExperienceYears = dto.ExperienceYears,
                ResumeLink = dto.ResumeLink,
                RegisteredAt = DateTime.UtcNow
            };

            await _repo.AddAsync(jobSeeker);

            return new GetJobSeekerDTO
            {
                JobSeekerId = jobSeeker.JobSeekerId,
                FullName = jobSeeker.FullName,
                Email = jobSeeker.Email,
                PhoneNumber = jobSeeker.PhoneNumber,
                Address = jobSeeker.Address,
                Skills = jobSeeker.Skills,
                ExperienceYears = jobSeeker.ExperienceYears,
                ResumeLink = jobSeeker.ResumeLink,
                RegisteredAt = jobSeeker.RegisteredAt
            };
        }

        public async Task<bool> DeleteJobSeekerAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<GetJobSeekerDTO>> GetAllJobSeekersAsync()
        {
            var seekers = await _repo.GetAllAsync();
            return seekers.Select(js => new GetJobSeekerDTO
            {
                JobSeekerId = js.JobSeekerId,
                FullName = js.FullName,
                Email = js.Email,
                PhoneNumber = js.PhoneNumber,
                Address = js.Address,
                Skills = js.Skills,
                ExperienceYears = js.ExperienceYears,
                ResumeLink = js.ResumeLink,
                RegisteredAt = js.RegisteredAt
            });
        }

        public async Task<GetJobSeekerDTO> GetJobSeekerByIdAsync(int id)
        {
            var js = await _repo.GetByIdAsync(id);
            if (js == null) return null!;

            return new GetJobSeekerDTO
            {
                JobSeekerId = js.JobSeekerId,
                FullName = js.FullName,
                Email = js.Email,
                PhoneNumber = js.PhoneNumber,
                Address = js.Address,
                Skills = js.Skills,
                ExperienceYears = js.ExperienceYears,
                ResumeLink = js.ResumeLink,
                RegisteredAt = js.RegisteredAt
            };
        }
    }
}
