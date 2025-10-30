import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Authservice } from '../Services/authservice';

export const RoleGuard: CanActivateFn = (route) => {
  const authService = inject(Authservice);
  const router = inject(Router);
  const expectedRole = route.data['role'];

  const userRole = authService.getRole();

  if (userRole !== expectedRole) {
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};
