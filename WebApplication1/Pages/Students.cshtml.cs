using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class StudentsModel : PageModel
{
    public List<StudentRow> Students { get; set; } = new();
    public int StudentCount { get; set; }

    [BindProperty] public int Id { get; set; }
    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public int? Dept { get; set; }
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

            const string getSql = "SELECT stud_id, stud_name, dept_id FROM Student WHERE stud_id=@id";
            using var cmd = new SqlCommand(getSql, con);
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                Id = r.GetInt32(0);
                Name = r.IsDBNull(1) ? "" : r.GetString(1);
                Dept = r.IsDBNull(2) ? null : r.GetInt32(2);
                EditMode = true;
            }
            else
            {
                Error = "Student not found.";
                EditMode = false;
            }
        }
        catch (SqlException ex)
        {
            Error = $"DB error while loading student: {ex.Message}";
        }

        LoadData();
    }

    public void OnPostSave()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Error = "Name is required.";
            LoadData();
            return;
        }
        if (Id <= 0)
        {
            Error = "Student Id must be a positive integer.";
            LoadData();
            return;
        }

        try
        {
            using var con = new SqlConnection(_cs);
            con.Open();

            if (EditMode)
            {
                const string upd = "UPDATE Student SET stud_name=@name, dept_id=@dept WHERE stud_id=@id";
                using var cmd = new SqlCommand(upd, con);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Parameters.AddWithValue("@name", Name);
                cmd.Parameters.AddWithValue("@dept", (object?)Dept ?? DBNull.Value);
                int rows = cmd.ExecuteNonQuery();
                Message = rows > 0 ? "Student updated." : "No changes made.";
            }
            else
            {
                const string ins = "INSERT INTO Student (stud_id, stud_name, dept_id) VALUES (@id, @name, @dept)";
                using var cmd = new SqlCommand(ins, con);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Parameters.AddWithValue("@name", Name);
                cmd.Parameters.AddWithValue("@dept", (object?)Dept ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                Message = "Student added.";

                Id = 0; Name = ""; Dept = null;
            }
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            Error = "A student with this Id already exists.";
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

            const string del = "DELETE FROM Student WHERE stud_id=@id";
            using var cmd = new SqlCommand(del, con);
            cmd.Parameters.AddWithValue("@id", id);
            int rows = cmd.ExecuteNonQuery();
            Message = rows > 0 ? "Student deleted." : "Student not found.";
        }
        catch (SqlException ex)
        {
            Error = $"Database error: {ex.Message}";
        }

        LoadData();
    }

    private void LoadData()
    {
        Students.Clear();

        try
        {
            using var con = new SqlConnection(_cs);
            con.Open();

            string list = @"
                SELECT s.stud_id, s.stud_name, s.dept_id, d.dept_name
                FROM Student s
                LEFT JOIN Department d ON s.dept_id = d.dept_id";

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                list += " WHERE s.stud_name LIKE '%' + @search + '%' OR d.dept_name LIKE '%' + @search + '%'";
            }

            list += " ORDER BY s.stud_id";

            using var cmd = new SqlCommand(list, con);
            if (!string.IsNullOrWhiteSpace(SearchTerm))
                cmd.Parameters.AddWithValue("@search", SearchTerm);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                Students.Add(new StudentRow
                {
                    Id = r.GetInt32(0),
                    Name = r.IsDBNull(1) ? "" : r.GetString(1),
                    DeptId = r.IsDBNull(2) ? null : r.GetInt32(2),
                    DeptName = r.IsDBNull(3) ? "" : r.GetString(3)
                });
            }
        }
        catch (SqlException ex)
        {
            Error = $"Database error: {ex.Message}";
        }

        StudentCount = Students.Count;
    }
}

public class StudentRow
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int? DeptId { get; set; }
    public string DeptName { get; set; } = "";
}
