import { TestBed } from '@angular/core/testing';

import { Applicationservice } from './applicationservice';

describe('Applicationservice', () => {
  let service: Applicationservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Applicationservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
