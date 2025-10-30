import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Apiservice } from '../Service/apiservice';
import { Doctor as DoctorModel } from '../Models/doctor.model';
import { Authservice } from '../Service/authservice';

@Component({
  selector: 'app-doctor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor.html',
  styleUrls: ['./doctor.css'],
})
export class Doctor implements OnInit {
  doctors: DoctorModel[] = [];
  editingDoctorId: number | null = null;
  confirmingDeleteId: number | null = null;

  newDoctor: Partial<DoctorModel> = {
    name: '',
    specialty: '',
    hospital: '',
  };

  constructor(private _apiservice: Apiservice,public auth: Authservice) {}

  ngOnInit(): void {
    this.loadDoctors();
  }

  loadDoctors(): void {
    this._apiservice.getAll().subscribe({
      next: (data) => (this.doctors = data),
      error: (err) => console.error('API Error:', err),
    });
  }

  addDoctor(): void {
    if (!this.newDoctor.name || !this.newDoctor.specialty || !this.newDoctor.hospital) {
      alert('Please fill in all fields!');
      return;
    }

    this._apiservice.adddoctor(this.newDoctor as DoctorModel).subscribe({
      next: (createdDoctor) => {
        this.doctors.push(createdDoctor);
        this.newDoctor = { name: '', specialty: '', hospital: '' };
        alert(`Doctor ${createdDoctor.name} added successfully!`);
      },
      error: (err) => console.error('Failed to add doctor:', err),
    });
  }

  startEdit(doctorId: number): void {
    this.editingDoctorId = doctorId;
  }

  cancelEdit(): void {
    this.editingDoctorId = null;
  }

  updateDoctor(doctor: DoctorModel): void {
    this._apiservice.updatedoctor(doctor).subscribe({
      next: (updatedDoctor) => {
        const index = this.doctors.findIndex((d) => d.doctorId === doctor.doctorId);
        if (index !== -1) {
          this.doctors[index] = { ...doctor };
        }
        alert(`Doctor ${doctor.name} updated successfully!`);

        this.editingDoctorId = null;
      },
      error: (err) => console.error('Failed to update doctor:', err),
    });
  }

  confirmDelete(doctorId: number): void {
    this.confirmingDeleteId = doctorId; 
  }

  cancelDelete(): void {
    this.confirmingDeleteId = null;
  }

  deleteDoctor(doctorId: number): void {
    this._apiservice.deletedoctor(doctorId).subscribe({
      next: () => {
        this.doctors = this.doctors.filter((d) => d.doctorId !== doctorId);
        alert('Doctor deleted successfully!');
        this.confirmingDeleteId = null;
      },
      error: (err) => console.error('Failed to delete doctor:', err),
    });
  }

}
