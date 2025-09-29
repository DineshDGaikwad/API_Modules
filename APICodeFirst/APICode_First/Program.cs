using CodeFirstAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CodeFirstAPI.Models;


var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Services
// -------------------------
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student API",
        Version = "v1",
        Description = "Code-First Student API"
    });
});

var app = builder.Build();

// -------------------------
// Middleware
// -------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
        c.RoutePrefix = "swagger"; // Swagger UI at /swagger
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // Enable attribute-routed controllers

// -------------------------
// Weather Forecast Endpoint
// -------------------------
string[] summaries =
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weather", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");


// GET all students
app.MapGet("/students", async (StudentDbContext context) =>
    await context.Students.ToListAsync());

// GET student by id
app.MapGet("/students/{id}", async (int id, StudentDbContext context) =>
{
    var student = await context.Students.FindAsync(id);
    return student is not null ? Results.Ok(student) : Results.NotFound();
});


// POST single student
app.MapPost("/students", async (Student student, StudentDbContext context) =>
{
    context.Students.Add(student);
    await context.SaveChangesAsync();
    return Results.Created($"/students/{student.StudentId}", student);
});

// POST multiple students (batch)
app.MapPost("/students/batch", async (List<Student> students, StudentDbContext context) =>
{
    context.Students.AddRange(students);
    await context.SaveChangesAsync();
    return Results.Created("/students", students);
});


// PUT student
app.MapPut("/students/{id}", async (int id, Student updatedStudent, StudentDbContext context) =>
{
    var student = await context.Students.FindAsync(id);
    if (student == null) return Results.NotFound();

    student.FullName = updatedStudent.FullName;
    student.Age = updatedStudent.Age;
    student.Email = updatedStudent.Email;
    student.Course = updatedStudent.Course;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE student
app.MapDelete("/students/{id}", async (int id, StudentDbContext context) =>
{
    var student = await context.Students.FindAsync(id);
    if (student == null) return Results.NotFound();

    context.Students.Remove(student);
    await context.SaveChangesAsync();
    return Results.NoContent();
});


app.Run();

// -------------------------
// Record: WeatherForecast
// -------------------------
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

