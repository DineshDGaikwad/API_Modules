import { Routes } from '@angular/router';
import { Doctor } from './doctor/doctor';
import { Logincomponent } from './logincomponent/logincomponent';
import { authgGuard } from './Guard/authg-guard';
import { Registercomponent } from './registercomponent/registercomponent';
import { rolegGuard } from './Guard/roleg-guard';

export const routes: Routes = [
  { path: 'login', component: Logincomponent },
  { path: 'register', component: Registercomponent },
  {
    path: 'doctors',
    component: Doctor,
    canActivate: [authgGuard, rolegGuard],
    data: { role: ['Admin', 'Doctor'] }
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' }
];
