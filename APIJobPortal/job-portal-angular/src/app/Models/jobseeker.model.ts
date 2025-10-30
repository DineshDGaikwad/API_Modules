export interface JobSeeker {
  jobSeekerId?: number;
  fullName: string;
  email: string;
  passwordHash?: string;
  phoneNumber?: string;
  address?: string;
  skills?: string;
  experienceYears?: number;
  resumeLink?: string;
}
