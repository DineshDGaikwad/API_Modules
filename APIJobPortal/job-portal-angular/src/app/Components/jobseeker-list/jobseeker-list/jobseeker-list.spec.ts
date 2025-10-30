import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobseekerList } from './jobseeker-list';

describe('JobseekerList', () => {
  let component: JobseekerList;
  let fixture: ComponentFixture<JobseekerList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobseekerList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobseekerList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
