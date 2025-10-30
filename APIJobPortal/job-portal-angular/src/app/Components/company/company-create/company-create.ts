import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CompanyService } from '../../../Services/companyservice';

@Component({
  selector: 'app-company-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './company-create.html',
  styleUrls: ['./company-create.css'] 
})
export class CompanyCreateComponent {
  companyForm: FormGroup;
  message: string = ''; 

  constructor(private fb: FormBuilder, private companyService: CompanyService) {
    this.companyForm = this.fb.group({
      name: ['', Validators.required],
      location: ['', Validators.required],
      industry: ['', Validators.required],
    });
  }

  createCompany(): void {
    if (this.companyForm.valid) {
      this.companyService.createCompany(this.companyForm.value).subscribe({
        next: (res) => {
          console.log(' Company created successfully', res);
          this.message = 'Company created successfully!';
          this.companyForm.reset();
        },
        error: (err) => {
          console.error('Error creating company', err);
          this.message = 'Error creating company. Please try again.';
        }
      });
    } else {
      this.message = 'Please fill all required fields.';
    }
  }
}
