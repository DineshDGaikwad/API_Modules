import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Authservice } from '../Service/authservice';

export const authgGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const authAuthservice = inject(Authservice);
  const loggedIn = authAuthservice.isLoggedIn();

  if (loggedIn) {
    return true;
  } else {
    router.navigate(['/login']);
    return false;
  }
};
