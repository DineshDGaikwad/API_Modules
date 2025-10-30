import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ApplicationService } from '../../../Services/applicationservice';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-application-create',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './application-create.html',
  styleUrls: ['./application-create.css']
})
export class ApplicationCreateComponent {
  applicationForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private appService: ApplicationService,
    private route: ActivatedRoute
  ) {
    this.applicationForm = this.fb.group({
      jobSeekerId: ['', Validators.required],
      jobId: ['', Validators.required],
      coverLetter: [''],
      resumeLink: ['']
    });
  }

  ngOnInit(): void {
    // Get jobId from query params and set it in the form
    this.route.queryParams.subscribe((params) => {
      if (params['jobId']) {
        this.applicationForm.patchValue({ jobId: params['jobId'] });
      }
    });
  }

  createApplication() {
    if (this.applicationForm.valid) {
      this.appService.createApplication(this.applicationForm.value).subscribe({
        next: (res) => {
          console.log('✅ Application submitted successfully', res);
          alert('Application submitted successfully!');
          this.applicationForm.reset();
        },
        error: (err) => {
          console.error('❌ Error submitting application', err);
          alert('Failed to submit application.');
        }
      });
    }
  }
}
