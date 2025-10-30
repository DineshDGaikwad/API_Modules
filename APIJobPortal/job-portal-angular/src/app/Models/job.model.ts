export interface Job {
  jobId?: number;
  title: string;
  description: string;
  location: string;
  type?: string;
  salary: number;
  postedDate?: Date;
  expiryDate?: Date;
  requiredExperience?: string;
}
