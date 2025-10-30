import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Jobseekerservice } from '../../../Services/jobseekerservice';

@Component({
  selector: 'app-jobseeker-list',
  standalone: true, 
  imports: [CommonModule], 
  templateUrl: './jobseeker-list.html',
  styleUrls: ['./jobseeker-list.css']
})
export class JobseekerList implements OnInit {
  jobSeekers: any[] = [];

  constructor(private Jobseekerservice: Jobseekerservice) {}

  ngOnInit(): void {
    this.loadJobSeekers();
  }

  loadJobSeekers(): void {
    this.Jobseekerservice.getAllJobSeekers().subscribe({
      next: (data) => (this.jobSeekers = data),
      error: (err) => console.error('Error loading job seekers:', err),
    });
  }
}
