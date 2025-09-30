using System.Collections.Generic;

namespace APIwithoutJunctionModel.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string Hospital { get; set; } = string.Empty;

        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}
