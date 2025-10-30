import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApplicationService } from '../../../Services/applicationservice';

@Component({
  selector: 'app-application-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './application-list.html',
  styleUrl: './application-list.css'
})
export class ApplicationListComponent implements OnInit {
  applications: any[] = [];

  constructor(private appService: ApplicationService) {}

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.appService.getAllApplications().subscribe({
      next: (res) => this.applications = res,
      error: (err) => console.error('Error fetching applications:', err)
    });
  }
}
