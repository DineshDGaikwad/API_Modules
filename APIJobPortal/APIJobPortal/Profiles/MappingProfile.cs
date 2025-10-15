using APIJobPortal.DTOs.Company;
using APIJobPortal.DTOs.Job;
using APIJobPortal.DTOs.Application;
using APIJobPortal.DTOs.JobSeeker;
using APIJobPortal.Models;
using AutoMapper;

namespace APIJobPortal.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, GetCompanyDTO>();
            CreateMap<CreateCompanyDTO, Company>();

            CreateMap<Job, GetJobDTO>();
            CreateMap<CreateJobDTO, Job>();

            CreateMap<JobApplication, GetApplicationDTO>();
            CreateMap<CreateApplicationDTO, JobApplication>();

            CreateMap<JobSeeker, GetJobSeekerDTO>();
            CreateMap<CreateJobSeekerDTO, JobSeeker>();
        }
    }
}
