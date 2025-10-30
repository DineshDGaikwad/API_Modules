import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompanyService } from '../../../Services/companyservice';
import { Company } from '../../../Models/company.model';
import { CompanyCreateComponent } from "../company-create/company-create";
import { RouterPreloader } from '@angular/router';

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [CommonModule, CompanyCreateComponent], 
  templateUrl: './company-list.html',
  styleUrls: ['./company-list.css'], 
})
export class CompanyListComponent implements OnInit {
  companies: Company[] = [];

  constructor(private companyService: CompanyService) {}

  ngOnInit(): void {
    this.loadCompanies();
  }

  loadCompanies(): void {
    this.companyService.getAllCompanies().subscribe({
      next: (data) => {
        console.log('Fetched companies:', data);
        this.companies = data;
      },
      error: (err) => {
        console.error('Error fetching companies:', err);
      }
    });
  }
}
