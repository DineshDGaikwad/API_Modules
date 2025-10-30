import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { JobService } from '../../../Services/jobservice';

@Component({
  selector: 'app-job-create',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './job-create.html',
  styleUrls: ['./job-create.css']
})
export class JobCreateComponent {
  jobForm: FormGroup;

  constructor(private fb: FormBuilder, private jobService: JobService) {
    this.jobForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      companyId: ['', Validators.required]
    });
  }

  createJob(): void {
    if (this.jobForm.valid) {
      this.jobService.createJob(this.jobForm.value).subscribe({
        next: (res) => {
          console.log('✅ Job created successfully:', res);
          alert('Job created successfully!');
          this.jobForm.reset();
        },
        error: (err) => {
          console.error('❌ Error creating job:', err);
          alert('Failed to create job.');
        }
      });
    } else {
      alert('Please fill all required fields.');
    }
  }
}
