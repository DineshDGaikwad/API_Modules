export interface JobApplication {
  applicationId?: number;
  jobSeekerId: number;
  jobId: number;
  applicationStatus?: string;
  resumeLink?: string;
  appliedOn?: Date;
  coverLetter?: string;
}
