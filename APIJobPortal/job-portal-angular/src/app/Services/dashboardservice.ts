import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private baseUrl = 'http://localhost:5066/api/Dashboard'; 

  constructor(private http: HttpClient) {}

  getSummary(): Observable<any> {
    return this.http.get(`${this.baseUrl}/summary`);
  }

  getCompanyApplications(): Observable<any> {
    return this.http.get(`${this.baseUrl}/company-applications`);
  }

  getJobApplications(): Observable<any> {
    return this.http.get(`${this.baseUrl}/job-applications`);
  }

  getTopSkills(): Observable<any> {
    return this.http.get(`${this.baseUrl}/top-skills`);
  }

  getApplicationTrends(): Observable<any> {
    return this.http.get(`${this.baseUrl}/application-trends`);
  }

  getRecentApplications(): Observable<any> {
    return this.http.get(`${this.baseUrl}/recent-applications`);
  }

  getRecentJobs(): Observable<any> {
  return this.http.get(`${this.baseUrl}/recent-jobs`);
}

}
