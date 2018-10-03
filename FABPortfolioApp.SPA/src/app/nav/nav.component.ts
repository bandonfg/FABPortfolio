import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { environment } from '../../environments/environment';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit, AfterViewInit {

  profilePic: string = environment.rootUrl + 'assets/images/profile.jpg';

  private fragment: string;
  loggedUser: any;

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService) {}

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

  loggedIn() {
    const token = localStorage.getItem('token');
    this.loggedUser = JSON.parse(localStorage.getItem('user'));
    return !!token;
  }

  logout() {
    this.authService.logout();
  }

}
