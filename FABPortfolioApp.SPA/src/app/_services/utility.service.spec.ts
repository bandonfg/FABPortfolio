/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { UtilityService } from './utility.service';

describe('Service: Utility', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [UtilityService]
    });
  });

  it('should ...', inject([UtilityService], (service: UtilityService) => {
    expect(service).toBeTruthy();
  }));
});
