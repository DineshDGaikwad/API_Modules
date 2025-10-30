import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JobService } from '../../../Services/jobservice';
import { Router } from '@angular/router';
import { JobCreateComponent } from "../job-create/job-create";

@Component({
  selector: 'app-job-list',
  standalone: true,
  imports: [CommonModule, JobCreateComponent],
  templateUrl: './job-list.html',
  styleUrls: ['./job-list.css']
})
export class JobListComponent implements OnInit {
  jobs: any[] = [];

  constructor(private jobService: JobService, private router: Router) {}

  ngOnInit(): void {
    this.loadJobs();
  }

  loadJobs(): void {
    this.jobService.getAllJobs().subscribe({
      next: (res) => (this.jobs = res),
      error: (err) => console.error('❌ Error fetching jobs:', err)
    });
  }

  applyForJob(jobId: number): void {
    this.router.navigate(['/apply'], { queryParams: { jobId } });
  }
}
