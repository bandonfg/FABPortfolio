import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';


import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  error = '';

  model: any = {};

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private alertify: AlertifyService) { }


  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

     // reset login status
     // this.authService.logout();

     // get return url from route parameters or default to '/'
     // this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

  } // end of ngOnInit

  // convenience getter for easy access to form fields
  get f() { return this.loginForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;

    this.model = {
      'username' : this.f.username.value,
      'password' : this.f.password.value
    };

    // this.f.username.value, this.f.password.value
    this.authService.login(this.model)
        .pipe(first())
        .subscribe(
            data => {
                // this.router.navigate([this.returnUrl]);
                this.router.navigate(['/home']);
                this.alertify.success('You have successfully logged-in.');
            },
            error => {
                this.error = error;
                this.loading = false;
                this.alertify.error('Access Denied - invalid username or password. Please try again.');
            });
  } // end of submit

}
