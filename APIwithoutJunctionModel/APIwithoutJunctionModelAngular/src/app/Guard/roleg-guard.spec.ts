import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { rolegGuard } from './roleg-guard';

describe('rolegGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => rolegGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
