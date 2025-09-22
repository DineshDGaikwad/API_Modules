using System.ComponentModel.DataAnnotations;

namespace CodeFirst.Models
{
    public class Product
    {
        [Key]
        public int pro_id { get; set; }
        public string pro_name { get; set; }
        public decimal price { get; set; }
    }
}