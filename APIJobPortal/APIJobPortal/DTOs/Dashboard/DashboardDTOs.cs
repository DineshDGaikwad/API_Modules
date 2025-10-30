public record KpiDto(string Name, long Value);
public record ApplicationsPerJobDto(int JobId, string JobTitle, int Applications);
public record StatusDistributionDto(string Status, int Count);
public record CompanyJobsDto(int CompanyId, string CompanyName, int TotalJobs, int TotalApplications);
public record JobsOverTimeDto(string Date, int JobsCreated);
