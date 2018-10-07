import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders, HttpRequest } from '@angular/common/http';
import { Router } from '../../../node_modules/@angular/router';
import {BehaviorSubject} from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';

import { JwtHelperService } from '@auth0/angular-jwt';
import { AlertifyService } from './alertify.service';
import { UserPW } from '../_models/user-pw';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + '/auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private router: Router) {
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
        }
      })
    );
  }

  register(user: User) {
    console.log('auth.service.register.email - posting user reg data ' + user.email);
    return this.http.post(this.baseUrl + 'register', user);
  }

  // PUT          api/auth/password
  // Description  user after login update password self service
  //              userPW: username, currentPassword, password
  updatePassword(userPW: UserPW) {
    const url = this.baseUrl + 'password';
    const params = new HttpParams();
    const headers = new HttpHeaders({ 'Authorization': 'Bearer ' + localStorage.getItem('token'), });
    const options = { headers: headers, params: params };
    const req = new HttpRequest('POST', url, userPW, options);
    return this.http.request(req);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.decodedToken = null;
    this.currentUser = null;
    this.alertify.message('You have successfully logged out');
    this.router.navigate(['/home']);
  }

  roleMatch(allowedRoles): boolean {
    let isMatch = false;
    const userRoles = this.decodedToken.role as Array<string>;
    allowedRoles.forEach(element => {
      if (userRoles.includes(element)) {
        isMatch = true;
        return;
      }
    });
    return isMatch;
  }

}
