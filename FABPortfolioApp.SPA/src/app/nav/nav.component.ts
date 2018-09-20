import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit, AfterViewInit {

  profilePic: string = environment.rootUrl + '/images/profile.jpg';

  private fragment: string;

  constructor(
    private route: ActivatedRoute) {}

  // init route fragment
  ngOnInit() {
    this.route.fragment.subscribe(fragment => { this.fragment = fragment; });
  }

  // scroll to specified element id
  ngAfterViewInit(): void {
    try {
      // original code to scroll without animation
      document.querySelector('#' + this.fragment).scrollIntoView();
    } catch (e) { }
  }
}
