import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Doctor } from '../Models/doctor.model';

@Injectable({
  providedIn: 'root'
})
export class Apiservice {
  private baseurl = 'http://localhost:5033/api/Doctors';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(this.baseurl);
  }

  adddoctor(doctor:Doctor):Observable<Doctor>{ 
    return this.http.post<Doctor>(this.baseurl, doctor)
  }

  updatedoctor(doctor: Doctor): Observable<Doctor> {
  return this.http.put<Doctor>(`${this.baseurl}/${doctor.doctorId}`, doctor);
}

  deletedoctor(doctorId: number): Observable<void> {
  return this.http.delete<void>(`${this.baseurl}/${doctorId}`);
}


}
