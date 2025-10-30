import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Authservice } from '../Service/authservice';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-registercomponent',
  imports: [FormsModule,RouterModule],
  templateUrl: './registercomponent.html',
  styleUrl: './registercomponent.css',
})
export class Registercomponent {
  userName = '';
  email = '';
  password = '';
  role = '';

  constructor(private auth: Authservice, private router: Router) {}

  registerUser() {
    if (!this.userName || !this.email || !this.password || !this.role) {
      alert('Please fill all fields.');
      return;
    }

    const newUser = {
  
      userName: this.userName,
      email: this.email,
      password: this.password,
      role: this.role,
    };

    this.auth.register(newUser).subscribe({
      next: (res) => {
        alert('Registration Successful');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        alert('Registration Failed. Please try again.');
        console.error('Registration Error:', err);
      },
    });
  }
}
