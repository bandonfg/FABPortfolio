import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, AfterViewInit {

  loggedUser: any;

  private fragment: string;
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private alertify: AlertifyService
) {}

  // init route fragment
  ngOnInit() {
    this.route.fragment.subscribe(fragment => { this.fragment = fragment; });
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    this.loggedUser = JSON.parse(localStorage.getItem('user'));
    return !!token;
  }

  logout() {
    this.authService.logout();
  }

  // scroll to provided element id
  ngAfterViewInit(): void {
    try {
      document.querySelector('#' + this.fragment).scrollIntoView();
    } catch (e) { }
  }
}
