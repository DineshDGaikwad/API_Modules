using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class DepartmentModel : PageModel
{
    public List<DepartmentRow> Departments { get; set; } = new();
    public int DepartmentCount { get; set; }

    [BindProperty] public int DeptId { get; set; }
    [BindProperty] public string DeptName { get; set; } = "";
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

            const string getSql = "SELECT dept_id, dept_name FROM Department WHERE dept_id=@id";
            using var cmd = new SqlCommand(getSql, con);
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                DeptId = r.GetInt32(0);
                DeptName = r.IsDBNull(1) ? "" : r.GetString(1);
                EditMode = true;
            }
            else
            {
                Error = "Department not found.";
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
        if (string.IsNullOrWhiteSpace(DeptName))
        {
            Error = "Department name is required.";
            LoadData();
            return;
        }
        if (DeptId <= 0)
        {
            Error = "Department Id must be positive.";
            LoadData();
            return;
        }

        try
        {
            using var con = new SqlConnection(_cs);
            con.Open();

            if (EditMode)
            {
                const string upd = "UPDATE Department SET dept_name=@name WHERE dept_id=@id";
                using var cmd = new SqlCommand(upd, con);
                cmd.Parameters.AddWithValue("@id", DeptId);
                cmd.Parameters.AddWithValue("@name", DeptName);
                int rows = cmd.ExecuteNonQuery();
                Message = rows > 0 ? "Department updated." : "No changes.";
            }
            else
            {
                const string ins = "INSERT INTO Department (dept_id, dept_name) VALUES (@id, @name)";
                using var cmd = new SqlCommand(ins, con);
                cmd.Parameters.AddWithValue("@id", DeptId);
                cmd.Parameters.AddWithValue("@name", DeptName);
                cmd.ExecuteNonQuery();
                Message = "Department added.";

                DeptId = 0; DeptName = "";
            }
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            Error = "A department with this Id already exists.";
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

            const string del = "DELETE FROM Department WHERE dept_id=@id";
            using var cmd = new SqlCommand(del, con);
            cmd.Parameters.AddWithValue("@id", id);
            int rows = cmd.ExecuteNonQuery();
            Message = rows > 0 ? "Department deleted." : "Not found.";
        }
        catch (SqlException ex)
        {
            Error = $"Database error: {ex.Message}";
        }

        LoadData();
    }

    private void LoadData()
    {
        Departments.Clear();

        try
        {
            using var con = new SqlConnection(_cs);
            con.Open();

            string list = @"
                SELECT dept_id, dept_name
                FROM Department
                WHERE (@search = '' OR @search IS NULL OR dept_name LIKE '%' + @search + '%')
                ORDER BY dept_id";

            using var cmd = new SqlCommand(list, con);
            cmd.Parameters.AddWithValue("@search", (object?)SearchTerm ?? "");

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                Departments.Add(new DepartmentRow
                {
                    DeptId = r.GetInt32(0),
                    DeptName = r.IsDBNull(1) ? "" : r.GetString(1)
                });
            }
        }
        catch (SqlException ex)
        {
            Error = $"Database error: {ex.Message}";
        }

        DepartmentCount = Departments.Count;
    }
}

public class DepartmentRow
{
    public int DeptId { get; set; }
    public string DeptName { get; set; } = "";
}
