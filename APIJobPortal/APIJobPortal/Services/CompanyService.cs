using APIJobPortal.DTOs.Company;
using APIJobPortal.Interfaces.Repositories;
using APIJobPortal.Interfaces.Services;
using APIJobPortal.Models;
using AutoMapper;

namespace APIJobPortal.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepo _companyRepo;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepo companyRepo, IMapper mapper)
        {
            _companyRepo = companyRepo;
            _mapper = mapper;
        }

        public async Task<GetCompanyDTO> CreateCompanyAsync(CreateCompanyDTO dto)
        {
            // AutoMapper converts DTO → Entity automatically
            var company = _mapper.Map<Company>(dto);
            company.CreatedAt = DateTime.UtcNow;

            await _companyRepo.AddAsync(company);

            // AutoMapper converts Entity → DTO automatically
            return _mapper.Map<GetCompanyDTO>(company);
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            return await _companyRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<GetCompanyDTO>> GetAllCompaniesAsync()
        {
            var companies = await _companyRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<GetCompanyDTO>>(companies);
        }

        public async Task<GetCompanyDTO?> GetCompanyByIdAsync(int id)
        {
            var company = await _companyRepo.GetByIdAsync(id);
            return company == null ? null : _mapper.Map<GetCompanyDTO>(company);
        }
    }
}
