using System.ComponentModel.DataAnnotations.Schema;

namespace APIJobPortal.Models
{
    public class CompanyJob
    {
        public int CompanyId { get; set; }
        public int JobId { get; set; }

        public Company Company { get; set; } = null!;
        public Job Job { get; set; } = null!;
    }
}
