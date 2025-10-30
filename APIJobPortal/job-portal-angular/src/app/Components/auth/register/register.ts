import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Authservice } from '../../../Services/authservice';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class RegisterComponent {
  registerForm: FormGroup;
  isCompany = false;

  constructor(private fb: FormBuilder, private authService: Authservice, private router: Router) {
    this.registerForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      password: ['', Validators.required],
      role: ['', Validators.required]
    });

    // Detect role change dynamically
    this.registerForm.get('role')?.valueChanges.subscribe(role => {
      this.isCompany = role === 'Company';
      const passwordControl = this.registerForm.get('password');

      if (this.isCompany) {
        passwordControl?.setValidators([Validators.required, Validators.pattern(/^[0-9]{10}$/)]);
      } else {
        passwordControl?.setValidators([Validators.required, Validators.minLength(5)]);
      }

      passwordControl?.updateValueAndValidity();
    });
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next: () => {
          alert('Registration successful! Please login.');
          this.router.navigate(['/login']);
        },
        error: () => alert('Error registering user.')
      });
    }
  }
}
