using System.Collections.Generic;

namespace APIwithoutJunctionModel.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Address { get; set; } = string.Empty;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
