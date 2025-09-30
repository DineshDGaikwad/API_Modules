using APIwithoutJunctionModel.Models;

namespace APIwithoutJunctionModel.Interfaces
{
    public interface IDoctorRepo
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
