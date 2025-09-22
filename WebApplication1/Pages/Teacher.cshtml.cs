using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class TeacherModel : PageModel
{
    public List<TeacherRow> Teachers { get; set; } = new();
    public int TeacherCount { get; set; }

    [BindProperty] public int TeacherId { get; set; }
    [BindProperty] public string TeacherName { get; set; } = "";
    [BindProperty] public int? DeptId { get; set; }
    [BindProperty] public bool EditMode { get; set; }

    public string? Message { get; set; }
    public string? Error { get; set; }


    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    private readonly string _cs =
        "Server=localhost,1433;Database=College;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;";

    public void OnGet() => LoadData();

    public void OnPostEdit(int id)
    {
        try
        {
            using var con = new SqlConnection(_cs);
            con.Open();

            const string getSql = "SELECT t_id, t_name, dept_id FROM Teacher WHERE t_id=@id";
            using var cmd = new SqlCommand(getSql, con);
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                TeacherId = r.GetInt32(0);
                TeacherName = r.IsDBNull(1) ? "" : r.GetString(1);
                DeptId = r.IsDBNull(2) ? null : r.GetInt32(2);
                EditMode = true;
            }
            else
            {
                Error = "Teacher not found.";
                EditMode = false;
            }
        }
        catch (SqlException ex)
        {
            Error = $"DB error: {ex.Message}";
        }

        LoadData();
    }

    public void OnPostSave()
    {
        if (string.IsNullOrWhiteSpace(TeacherName))
        {
            Error = "Teacher name is required.";
            LoadData();
            return;
        }
        if (TeacherId <= 0)
        {
            Error = "Teacher Id must be positive.";
            LoadData();
            return;
        }

        try
        {
            using var con = new SqlConnection(_cs);
            con.Open();

            if (EditMode)
            {
                const string upd = "UPDATE Teacher SET t_name=@name, dept_id=@dept WHERE t_id=@id";
                using var cmd = new SqlCommand(upd, con);
                cmd.Parameters.AddWithValue("@id", TeacherId);
                cmd.Parameters.AddWithValue("@name", TeacherName);
                cmd.Parameters.AddWithValue("@dept", (object?)DeptId ?? DBNull.Value);
                int rows = cmd.ExecuteNonQuery();
                Message = rows > 0 ? "Teacher updated." : "No changes.";
            }
            else
            {
                const string ins = "INSERT INTO Teacher (t_id, t_name, dept_id) VALUES (@id, @name, @dept)";
                using var cmd = new SqlCommand(ins, con);
                cmd.Parameters.AddWithValue("@id", TeacherId);
                cmd.Parameters.AddWithValue("@name", TeacherName);
                cmd.Parameters.AddWithValue("@dept", (object?)DeptId ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                Message = "Teacher added.";

                TeacherId = 0; TeacherName = ""; DeptId = null;
            }
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            Error = "A teacher with this Id already exists.";
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            Error = "Invalid Department Id (foreign key constraint).";
        }
        catch (SqlException ex)
        {
            Error = $"Database error: {ex.Message}";
        }

        EditMode = false;
        LoadData();
    }

    public void OnPostDelete(int id)
    {
        try
        {
            using var con = new SqlConnection(_cs);
            con.Open();

            const string del = "DELETE FROM Teacher WHERE t_id=@id";
            using var cmd = new SqlCommand(del, con);
            cmd.Parameters.AddWithValue("@id", id);
            int rows = cmd.ExecuteNonQuery();
            Message = rows > 0 ? "Teacher deleted." : "Not found.";
        }
        catch (SqlException ex)
        {
            Error = $"Database error: {ex.Message}";
        }

        LoadData();
    }

    private void LoadData()
    {
        Teachers.Clear();

        try
        {
            using var con = new SqlConnection(_cs);
            con.Open();

            string list = @"
                SELECT t.t_id, t.t_name, t.dept_id, d.dept_name
                FROM Teacher t
                LEFT JOIN Department d ON t.dept_id = d.dept_id
                WHERE (@search = '' OR @search IS NULL 
                       OR t.t_name LIKE '%' + @search + '%' 
                       OR d.dept_name LIKE '%' + @search + '%')
                ORDER BY t.t_id";

            using var cmd = new SqlCommand(list, con);
            cmd.Parameters.AddWithValue("@search", (object?)SearchTerm ?? "");

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                Teachers.Add(new TeacherRow
                {
                    TeacherId = r.GetInt32(0),
                    TeacherName = r.IsDBNull(1) ? "" : r.GetString(1),
                    DeptId = r.IsDBNull(2) ? null : r.GetInt32(2),
                    DeptName = r.IsDBNull(3) ? "" : r.GetString(3)
                });
            }
        }
        catch (SqlException ex)
        {
            Error = $"Database error: {ex.Message}";
        }

        TeacherCount = Teachers.Count;
    }
}

public class TeacherRow
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = "";
    public int? DeptId { get; set; }
    public string DeptName { get; set; } = "";
}
