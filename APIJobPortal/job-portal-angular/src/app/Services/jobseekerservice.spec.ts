import { TestBed } from '@angular/core/testing';

import { Jobseekerservice } from './jobseekerservice';

describe('Jobseekerservice', () => {
  let service: Jobseekerservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Jobseekerservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
