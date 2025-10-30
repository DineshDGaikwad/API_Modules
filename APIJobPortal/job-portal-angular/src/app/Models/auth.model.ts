export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;  // For JobSeeker: password; For Company: mobile number
  role: string;      // "JobSeeker" or "Company"
}

export interface LoginRequest {
  email: string;
  password: string;  // For Company: enter mobile number here
}

export interface AuthResponse {
  token: string;
  role: string;
  userId: number;
  email: string;
}
