using APIJobPortal.Data;
using APIJobPortal.Interfaces.Repositories;
using APIJobPortal.Interfaces.Services;
using APIJobPortal.Repository;
using APIJobPortal.Repositories;
using APIJobPortal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<JobPortalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repos & Services
builder.Services.AddScoped<IJobSeekerRepo, JobSeekerRepository>();
builder.Services.AddScoped<IJobRepo, JobRepository>();
builder.Services.AddScoped<ICompanyRepo, CompanyRepository>();
builder.Services.AddScoped<IApplicationRepo, ApplicationRepository>();

builder.Services.AddScoped<IJobSeekerService, JobSeekerService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAutoMapper(typeof(Program));

// JWT settings
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secret = jwtSettings["Secret"] ?? throw new Exception("Jwt:Secret missing in configuration");
// ensure secret length is sufficient:
if (Encoding.UTF8.GetBytes(secret).Length < 32)
    throw new Exception("Jwt:Secret must be at least 32 bytes (e.g. 32+ characters). Use a longer secret for HS256.");

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // set true in prod
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = !string.IsNullOrEmpty(jwtSettings["Issuer"]),
        ValidateAudience = !string.IsNullOrEmpty(jwtSettings["Audience"]),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    // helpful debugging logs for auth issues
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ctx.Exception, "Authentication failed.");
            return Task.CompletedTask;
        },
        OnTokenValidated = ctx =>
        {
            return Task.CompletedTask;
        }
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIJobPortal", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token as: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // <<< must be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
