using APIwithoutJunctionModel.Data;
using APIwithoutJunctionModel.Models;
using APIwithoutJunctionModel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APIwithoutJunctionModel.Repository
{
    public class DoctorRepository : IDoctorRepo
    {
        private readonly DocPatDbContext _context;

        public DoctorRepository(DocPatDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.Patients)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.Patients)
                .FirstOrDefaultAsync(d => d.DoctorId == id);
        }

        public async Task AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
