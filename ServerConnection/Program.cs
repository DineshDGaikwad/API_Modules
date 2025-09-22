using System;
using Microsoft.Data.SqlClient;

class Program
{
    static readonly string connectionString =
        "Server=localhost,1433;Database=College;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=False;";

    static void Main(string[] args)
    {
        string choice = "yes";
        while (choice.Equals("yes", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\nEnter the operation to be performed:");
            Console.WriteLine("1. Create Student Table");
            Console.WriteLine("2. Insert Student Record");
            Console.WriteLine("3. Display Student Records");
            Console.WriteLine("4. Create Course Table");
            Console.WriteLine("5. Insert Course Record");
            Console.WriteLine("6. Display Course Records");
            Console.Write("👉 Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("⚠️ Invalid input. Please enter a number.");
                continue;
            }

            try
            {
                switch (option)
                {
                    case 1: CreateStudent(); break;
                    case 2: InsertStudent(); break;
                    case 3: DisplayStudent(); break;
                    case 4: CreateCourse(); break;
                    case 5: InsertCourse(); break;
                    case 6: DisplayCourse(); break;
                    default: Console.WriteLine("⚠️ Invalid choice."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }

            Console.Write("\nDo you want to continue? (yes/no): ");
            choice = Console.ReadLine() ?? "no";
        }
    }

    static void CreateStudent()
    {
        string query = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Student' AND xtype='U')
            CREATE TABLE Student (
                stud_id INT PRIMARY KEY,
                stud_name NVARCHAR(100),
                dept_id INT
            );";

        ExecuteNonQuery(query, "✅ Student Table created (if not exists).");
    }

    static void InsertStudent()
    {
        Console.Write("Enter Student ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        Console.Write("Enter Student Name: ");
        string name = Console.ReadLine() ?? "Unknown";

        Console.Write("Enter Department ID: ");
        if (!int.TryParse(Console.ReadLine(), out int deptId)) return;

        string query = "INSERT INTO Student VALUES (@id, @name, @deptId)";

        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@deptId", deptId);
        cmd.ExecuteNonQuery();

        Console.WriteLine("✅ Student inserted successfully.");
    }

    static void DisplayStudent()
    {
        string query = "SELECT * FROM Student";

        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand cmd = new SqlCommand(query, con);
        using SqlDataReader reader = cmd.ExecuteReader();

        Console.WriteLine("\n📊 Student Records:");
        Console.WriteLine("stud_id\tstud_name\tdept_id");
        Console.WriteLine("----------------------------------");

        while (reader.Read())
        {
            Console.WriteLine($"{reader["stud_id"]}\t{reader["stud_name"]}\t{reader["dept_id"]}");
        }
    }

    static void CreateCourse()
    {
        string query = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Course' AND xtype='U')
            CREATE TABLE Course (
                course_id INT PRIMARY KEY,
                course_name NVARCHAR(100),
                dept_id INT,
                t_id INT
            );";

        ExecuteNonQuery(query, "✅ Course Table created (if not exists).");
    }

    static void InsertCourse()
    {
        Console.Write("Enter Course ID: ");
        if (!int.TryParse(Console.ReadLine(), out int cid)) return;

        Console.Write("Enter Course Name: ");
        string cname = Console.ReadLine() ?? "Unknown";

        Console.Write("Enter Department ID: ");
        if (!int.TryParse(Console.ReadLine(), out int deptId)) return;

        Console.Write("Enter Teacher ID: ");
        if (!int.TryParse(Console.ReadLine(), out int tid)) return;

        string query = "INSERT INTO Course VALUES (@cid, @cname, @deptId, @tid)";

        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@cid", cid);
        cmd.Parameters.AddWithValue("@cname", cname);
        cmd.Parameters.AddWithValue("@deptId", deptId);
        cmd.Parameters.AddWithValue("@tid", tid);
        cmd.ExecuteNonQuery();

        Console.WriteLine("✅ Course inserted successfully.");
    }

    static void DisplayCourse()
    {
        string query = "SELECT * FROM Course";

        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand cmd = new SqlCommand(query, con);
        using SqlDataReader reader = cmd.ExecuteReader();

        Console.WriteLine("\n📊 Course Records:");
        Console.WriteLine("course_id\tcourse_name\tdept_id\tt_id");
        Console.WriteLine("------------------------------------------------");

        while (reader.Read())
        {
            Console.WriteLine($"{reader["course_id"]}\t{reader["course_name"]}\t{reader["dept_id"]}\t{reader["t_id"]}");
        }
    }

    static void ExecuteNonQuery(string query, string successMsg)
    {
        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand cmd = new SqlCommand(query, con);
        cmd.ExecuteNonQuery();
        Console.WriteLine(successMsg);
    }
}
