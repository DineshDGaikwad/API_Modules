import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Jobseekerservice {
  private baseUrl = 'http://localhost:5066/api/JobSeekers';

  constructor(private http: HttpClient) {}

  getAllJobSeekers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}`);
  }

  getJobSeekerById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${id}`);
  }

  createJobSeeker(jobSeeker: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}`, jobSeeker);
  }

  deleteJobSeeker(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
