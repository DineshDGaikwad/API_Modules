using APIJobPortal.DTOs.Company;

namespace APIJobPortal.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<GetCompanyDTO> GetCompanyByIdAsync(int id);
        Task<IEnumerable<GetCompanyDTO>> GetAllCompaniesAsync();
        Task<GetCompanyDTO> CreateCompanyAsync(CreateCompanyDTO dto);
        Task<bool> DeleteCompanyAsync(int id);
    }
}
