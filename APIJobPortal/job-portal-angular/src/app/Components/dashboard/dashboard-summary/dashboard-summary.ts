import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../../../Services/dashboardservice';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard-summary',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard-summary.html',
  styleUrls: ['./dashboard-summary.css'],
})
export class DashboardSummaryComponent implements OnInit {
  summary: any = {};
  recentApplications: any[] = [];
    recentJobs: any[] = [];


  @ViewChild('companyChart') companyChartRef!: ElementRef;
  @ViewChild('trendChart') trendChartRef!: ElementRef;
  @ViewChild('skillsChart') skillsChartRef!: ElementRef;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadSummary();
    this.loadCharts();
    this.loadRecentApplications();
     this.loadRecentJobs();
  }

  loadSummary(): void {
    this.dashboardService.getSummary().subscribe({
      next: (data) => (this.summary = data),
      error: (err) => console.error('Error fetching summary', err),
    });
  }

  loadCharts(): void {
    this.dashboardService.getCompanyApplications().subscribe((data) => {
      const labels = data.map((x: any) => x.companyName);
      const values = data.map((x: any) => x.totalApplications);
      new Chart(this.companyChartRef.nativeElement, {
        type: 'bar',
        data: {
          labels,
          datasets: [
            {
              label: 'Applications per Company',
              data: values,
              backgroundColor: '#4158d0aa',
            },
          ],
        },
        options: {
          responsive: true,
          plugins: { legend: { display: false } },
        },
      });
    });

    this.dashboardService.getApplicationTrends().subscribe((data) => {
      const labels = data.map((x: any) => x.month);
      const values = data.map((x: any) => x.count);
      new Chart(this.trendChartRef.nativeElement, {
        type: 'line',
        data: {
          labels,
          datasets: [
            {
              label: 'Monthly Applications',
              data: values,
              borderColor: '#1f3b73',
              fill: false,
              tension: 0.3,
            },
          ],
        },
        options: {
          responsive: true,
        },
      });
    });

    this.dashboardService.getTopSkills().subscribe((data) => {
      const labels = data.map((x: any) => x.skill);
      const values = data.map((x: any) => x.count);
      new Chart(this.skillsChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels,
          datasets: [
            {
              label: 'Top Skills',
              data: values,
              backgroundColor: [
                '#4158d0',
                '#2b91e3',
                '#1f3b73',
                '#8ecae6',
                '#023047',
                '#ffb703',
                '#fb8500',
              ],
            },
          ],
        },
        options: {
          responsive: true,
          plugins: { legend: { position: 'bottom' } },
        },
      });
    });
  }

  loadRecentApplications(): void {
    this.dashboardService.getRecentApplications().subscribe((data) => {
      this.recentApplications = data;
    });
  }

  loadRecentJobs(): void {
    this.dashboardService.getRecentJobs().subscribe((data) => {
      this.recentJobs = data;
    });
  }
  
}
