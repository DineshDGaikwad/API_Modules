using APIwithoutJunctionModel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using APIwithoutJunctionModel.Interfaces;
using APIwithoutJunctionModel.Repository;
using APIwithoutJunctionModel.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DocPatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDoctorRepo, DoctorRepository>();

builder.Services.AddScoped<IDoctorService, DoctorService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DoctorPatientAPI", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DoctorPatientAPI v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 7️⃣ Run App
app.Run();
