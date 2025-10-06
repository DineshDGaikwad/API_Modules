using System.ComponentModel.DataAnnotations;

namespace APIwithoutJunctionModel.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }
        public string? userName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
    }
}