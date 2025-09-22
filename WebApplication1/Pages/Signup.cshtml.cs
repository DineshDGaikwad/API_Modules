using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace YourApp.Pages
{
    public class SignupModel : PageModel
    {
        private readonly string connectionString =
            "Server=localhost,1433;Database=College;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;";

        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public string Email { get; set; }

        public string Message { get; set; }
        public bool IsSuccess { get; set; } = false;   // ✅ Added

        public void OnGet() { }

        public IActionResult OnPost()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM Users WHERE username=@username";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@username", Username);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        Message = "⚠️ Username already exists.";
                        return Page();
                    }
                }

                string insertQuery = "INSERT INTO Users (username, password, email) VALUES (@username, @password, @email)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@username", Username);
                    cmd.Parameters.AddWithValue("@password", Password); // ⚠️ later use hash
                    cmd.Parameters.AddWithValue("@email", Email);
                    cmd.ExecuteNonQuery();
                }
            }

            IsSuccess = true;
            return Page();
        }
    }
}
