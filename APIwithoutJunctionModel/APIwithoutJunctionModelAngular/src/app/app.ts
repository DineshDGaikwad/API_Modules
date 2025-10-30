import { Component, signal } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Doctor } from './doctor/doctor';
import { HttpClient } from '@angular/common/http';
import { routes } from './app.routes';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ RouterModule, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('APIwithoutJunctionModelAngular');
}
