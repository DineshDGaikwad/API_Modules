import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Authservice } from '../Service/authservice';

@Component({
  selector: 'app-logincomponent',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './logincomponent.html',
  styleUrl: './logincomponent.css',
})
export class Logincomponent {
  email = '';
  password = '';
  role = '';
  message = '';

  constructor(private auth: Authservice, private router: Router) {}

  loginUser() {
    if (!this.email || !this.password || !this.role) {
      this.message = 'Please fill all fields.';
      return;
    }

    this.auth.login(this.email, this.password, this.role).subscribe({
      next: (res) => {
        this.message = 'Login Successful';
        localStorage.setItem('token', res.token);
        localStorage.setItem('role', res.role);
        this.router.navigate(['/doctors']);
      },
      error: (err) => {
        this.message = 'Login Failed. Please check credentials.';
        console.error('Login Error:', err);
      },
    });
  }
}
