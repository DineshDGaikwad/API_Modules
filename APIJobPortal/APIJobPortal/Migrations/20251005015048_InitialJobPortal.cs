using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIJobPortal.Migrations
{
    /// <inheritdoc />
    public partial class InitialJobPortal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiredExperience = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.JobId);
                });

            migrationBuilder.CreateTable(
                name: "JobSeekers",
                columns: table => new
                {
                    JobSeekerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Skills = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    ResumeLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekers", x => x.JobSeekerId);
                });

            migrationBuilder.CreateTable(
                name: "CompanyJobs",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyJobs", x => new { x.CompanyId, x.JobId });
                    table.ForeignKey(
                        name: "FK_CompanyJobs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyJobs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    JobSeekerId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResumeLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppliedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CoverLetter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => new { x.JobId, x.JobSeekerId });
                    table.ForeignKey(
                        name: "FK_JobApplications_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_JobApplications_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "JobSeekerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplications_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "CompanyId", "ContactNumber", "CreatedAt", "Email", "Industry", "Location", "Name", "Website" },
                values: new object[,]
                {
                    { 1, "1234567890", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "contact@technova.com", "IT Services", "Pune", "TechNova Pvt Ltd", "https://www.technova.com" },
                    { 2, "9876543210", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "info@healthlink.com", "Healthcare", "Bangalore", "HealthLink", "https://www.healthlink.com" }
                });

            migrationBuilder.InsertData(
                table: "JobSeekers",
                columns: new[] { "JobSeekerId", "Address", "Email", "ExperienceYears", "FullName", "PasswordHash", "PhoneNumber", "RegisteredAt", "ResumeLink", "Skills" },
                values: new object[,]
                {
                    { 1, null, "alice@example.com", 0, "Alice Johnson", "hashedpassword1", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, null, "bob@example.com", 0, "Bob Smith", "hashedpassword2", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null }
                });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "JobId", "Description", "ExpiryDate", "Location", "PostedDate", "RequiredExperience", "Salary", "Title", "Type" },
                values: new object[,]
                {
                    { 1, "Full Stack Developer", null, "Remote", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 85000m, "Software Developer", "Full-Time" },
                    { 2, "Analyze large datasets", null, "Mumbai", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 65000m, "Data Analyst", "Full-Time" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobs_JobId",
                table: "CompanyJobs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_CompanyId",
                table: "JobApplications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobSeekerId",
                table: "JobApplications",
                column: "JobSeekerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyJobs");

            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "JobSeekers");

            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
