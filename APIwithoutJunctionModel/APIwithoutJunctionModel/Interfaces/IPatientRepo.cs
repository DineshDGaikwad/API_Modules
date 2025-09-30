using APIwithoutJunctionModel.Models;

namespace APIwithoutJunctionModel.Interfaces
{
    public interface IPatientRepo
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
