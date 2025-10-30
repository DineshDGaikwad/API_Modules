import { Routes } from '@angular/router';
import { CompanyListComponent } from './Components/company/company-list/company-list';
import { JobListComponent } from './Components/job/job-list/job-list';
import { DashboardSummaryComponent } from './Components/dashboard/dashboard-summary/dashboard-summary';
import { ApplicationListComponent } from './Components/application/application-list/application-list';
import { ApplicationCreateComponent } from './Components/application/application-create/application-create';
import { JobCreateComponent } from './Components/job/job-create/job-create';
import { JobseekerList } from './Components/jobseeker-list/jobseeker-list/jobseeker-list';
import { LoginComponent } from './Components/auth/login/login';
import { RegisterComponent } from './Components/auth/register/register';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'companies', component: CompanyListComponent },
  { path: 'jobs', component: JobListComponent },
  { path: 'dashboard', component: DashboardSummaryComponent },
  { path: 'applications', component: ApplicationListComponent },
  { path: 'applications/create', component: ApplicationCreateComponent },
  { path: 'jobs', component: JobListComponent },
  { path: 'jobs/create', component: JobCreateComponent },
  { path: 'JobSeekers', component: JobseekerList },
  { path: 'apply', component: ApplicationCreateComponent }
];
