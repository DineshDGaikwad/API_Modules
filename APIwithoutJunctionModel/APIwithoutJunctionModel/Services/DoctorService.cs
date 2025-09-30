using APIwithoutJunctionModel.DTOs;
using APIwithoutJunctionModel.Models;
using APIwithoutJunctionModel.Interfaces;
using APIwithoutJunctionModel.Repository;
using APIwithoutJunctionModel.Services;

namespace APIwithoutJunctionModel.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepo _doctorRepo;

        public DoctorService(IDoctorRepo doctorRepo)
        {
            _doctorRepo = doctorRepo;
        }

        public async Task<IEnumerable<GetDocDTO>> GetAllAsync()
        {
            var doctors = await _doctorRepo.GetAllAsync();
            return doctors.Select(d => new GetDocDTO
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Specialty = d.Specialty,
                Hospital = d.Hospital,
                Patients = d.Patients.Select(p => p.Name).ToList()
            });
        }

        public async Task<GetDocDTO?> GetByIdAsync(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null) return null;

            return new GetDocDTO
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Specialty = doctor.Specialty,
                Hospital = doctor.Hospital,
                Patients = doctor.Patients.Select(p => p.Name).ToList()
            };
        }

        public async Task<GetDocDTO> CreateAsync(CreateDocDTO dto)
        {
            var doctor = new Doctor
            {
                Name = dto.Name,
                Specialty = dto.Specialty,
                Hospital = dto.Hospital
            };

            await _doctorRepo.AddAsync(doctor);
            await _doctorRepo.SaveChangesAsync();

            return new GetDocDTO
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Specialty = doctor.Specialty,
                Hospital = doctor.Hospital,
                Patients = new List<string>()
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateDocDTO dto)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null) return false;

            doctor.Name = dto.Name;
            doctor.Specialty = dto.Specialty;
            doctor.Hospital = dto.Hospital;

            await _doctorRepo.UpdateAsync(doctor);
            await _doctorRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null) return false;

            await _doctorRepo.DeleteAsync(id);
            await _doctorRepo.SaveChangesAsync();
            return true;
        }
    }
}
