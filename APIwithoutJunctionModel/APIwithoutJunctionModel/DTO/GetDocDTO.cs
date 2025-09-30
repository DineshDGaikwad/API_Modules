namespace APIwithoutJunctionModel.DTOs
{
    public class GetDocDTO
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string Hospital { get; set; } = string.Empty;

        public List<string> Patients { get; set; } = new();
    }
}
