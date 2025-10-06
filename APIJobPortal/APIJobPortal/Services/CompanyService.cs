using APIJobPortal.DTOs.Company;
using APIJobPortal.Interfaces.Services;
using APIJobPortal.Models;
using APIJobPortal.Interfaces.Repositories;

namespace APIJobPortal.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepo _companyRepo;

        public CompanyService(ICompanyRepo companyRepo)
        {
            _companyRepo = companyRepo;
        }

        public async Task<GetCompanyDTO> CreateCompanyAsync(CreateCompanyDTO dto)
        {
            var company = new Company
            {
                Name = dto.Name,
                Industry = dto.Industry,
                Location = dto.Location,
                Email = dto.Email,
                ContactNumber = dto.ContactNumber,
                Website = dto.Website,
                CreatedAt = DateTime.UtcNow
            };

            await _companyRepo.AddAsync(company);

            return new GetCompanyDTO
            {
                CompanyId = company.CompanyId,
                Name = company.Name,
                Industry = company.Industry,
                Location = company.Location,
                Email = company.Email,
                ContactNumber = company.ContactNumber,
                Website = company.Website,
                CreatedAt = company.CreatedAt
            };
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            return await _companyRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<GetCompanyDTO>> GetAllCompaniesAsync()
        {
            var companies = await _companyRepo.GetAllAsync();
            return companies.Select(c => new GetCompanyDTO
            {
                CompanyId = c.CompanyId,
                Name = c.Name,
                Industry = c.Industry,
                Location = c.Location,
                Email = c.Email,
                ContactNumber = c.ContactNumber,
                Website = c.Website,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<GetCompanyDTO?> GetCompanyByIdAsync(int id)
        {
            var company = await _companyRepo.GetByIdAsync(id);
            if (company == null) return null;

            return new GetCompanyDTO
            {
                CompanyId = company.CompanyId,
                Name = company.Name,
                Industry = company.Industry,
                Location = company.Location,
                Email = company.Email,
                ContactNumber = company.ContactNumber,
                Website = company.Website,
                CreatedAt = company.CreatedAt
            };
        }
    }
}
