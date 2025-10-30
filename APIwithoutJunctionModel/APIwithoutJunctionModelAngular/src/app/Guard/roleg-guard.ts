import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Authservice } from '../Service/authservice';

export const rolegGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const authservice = inject(Authservice);

  const allowedRoles = route.data['role'] as string[]; // now expects an array
  const userRole = authservice.getRole();

  // User must be logged in and have one of the allowed roles
  if (authservice.isLoggedIn() && allowedRoles.includes(userRole!)) {
    return true;
  } else {
    router.navigate(['/login']);
    return false;
  }
};
