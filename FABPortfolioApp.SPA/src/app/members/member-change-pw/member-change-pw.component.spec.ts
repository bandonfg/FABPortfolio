/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MemberChangePWComponent } from './member-change-pw.component';

describe('MemberChangePWComponent', () => {
  let component: MemberChangePWComponent;
  let fixture: ComponentFixture<MemberChangePWComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MemberChangePWComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberChangePWComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
