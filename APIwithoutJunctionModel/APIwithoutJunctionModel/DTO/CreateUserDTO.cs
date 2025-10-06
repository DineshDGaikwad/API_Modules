namespace APIwithoutJunctionModel.DTOs
{
    public class CreateUserDTO
    {
        public string? userName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
    }
}