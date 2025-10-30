import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { user } from '../Models/user.model';

@Injectable({
  providedIn: 'root',
})
export class Authservice {
  private baseurl = 'http://localhost:5033/api';

  constructor(private http: HttpClient) {}

  login(email: string, password: string, role: string): Observable<{ token: string; role: string }> {
    const body = { email, password, role };
    return this.http.post<{ token: string; role: string }>(`${this.baseurl}/Token`, body).pipe(
      tap((response) => {
        localStorage.setItem('authtoken', response.token);
        localStorage.setItem('role', response.role);
      })
    );
  }

  register(user:user): Observable<user> {
    return this.http.post<user>(`${this.baseurl}/User`, user);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('authtoken');
  }

  getRole(){
    return localStorage.getItem('role');
  }

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  isDoctor(): boolean {
    return this.getRole() === 'Doctor';
  }

}
