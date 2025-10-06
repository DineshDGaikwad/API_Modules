using APIJobPortal.DTOs.Application;
using APIJobPortal.Interfaces.Services;
using APIJobPortal.Models;
using APIJobPortal.Interfaces.Repositories;

namespace APIJobPortal.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepo _applicationRepo;

        public ApplicationService(IApplicationRepo applicationRepo)
        {
            _applicationRepo = applicationRepo;
        }

        public async Task<GetApplicationDTO> CreateApplicationAsync(CreateApplicationDTO dto)
        {
            var application = new JobApplication
            {
                JobId = dto.JobId,
                JobSeekerId = dto.JobSeekerId,
                ApplicationStatus = "Pending",
                ResumeLink = dto.ResumeLink,
                AppliedOn = DateTime.UtcNow,
                CoverLetter = dto.CoverLetter
            };

            await _applicationRepo.AddAsync(application);

            return new GetApplicationDTO
            {
                ApplicationId = application.ApplicationId,
                JobId = application.JobId,
                JobSeekerId = application.JobSeekerId,
                ApplicationStatus = application.ApplicationStatus,
                ResumeLink = application.ResumeLink,
                AppliedOn = application.AppliedOn,
                CoverLetter = application.CoverLetter
            };
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            return await _applicationRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<GetApplicationDTO>> GetAllApplicationsAsync()
        {
            var applications = await _applicationRepo.GetAllAsync();
            return applications.Select(a => new GetApplicationDTO
            {
                ApplicationId = a.ApplicationId,
                JobId = a.JobId,
                JobSeekerId = a.JobSeekerId,
                ApplicationStatus = a.ApplicationStatus,
                ResumeLink = a.ResumeLink,
                AppliedOn = a.AppliedOn,
                CoverLetter = a.CoverLetter
            });
        }

        public async Task<GetApplicationDTO?> GetApplicationByIdAsync(int id)
        {
            var application = await _applicationRepo.GetByIdAsync(id);
            if (application == null) return null;

            return new GetApplicationDTO
            {
                ApplicationId = application.ApplicationId,
                JobId = application.JobId,
                JobSeekerId = application.JobSeekerId,
                ApplicationStatus = application.ApplicationStatus,
                ResumeLink = application.ResumeLink,
                AppliedOn = application.AppliedOn,
                CoverLetter = application.CoverLetter
            };
        }
    }
}
