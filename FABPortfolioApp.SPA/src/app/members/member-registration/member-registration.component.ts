import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { first } from 'rxjs/operators';
import { User } from '../../_models/user';


@Component({
  selector: 'app-member-registration',
  templateUrl: './member-registration.component.html',
  styleUrls: ['./member-registration.component.css']
})
export class MemberRegistrationComponent implements OnInit {

  regForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  user: User;

  model: any = {};

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    // register form input validators
    this.regForm = this.formBuilder.group({
      firstName:  ['', [ Validators.required, Validators.minLength(2), Validators.maxLength(20) ]],
      lastName:   ['', [ Validators.required, Validators.minLength(2), Validators.maxLength(20)]],
      username:   [''],
      email:      ['', [ Validators.required, Validators.email]],
      password:   ['', [ Validators.required, Validators.minLength(6), Validators.maxLength(20) ]],
      confirmPassword:  ['', Validators.required]

    }, {
        validator: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

  get f() { return this.regForm.controls; }

  onSubmit() {
    // submit entries when form is valid
    if (this.regForm.valid) {
      this.submitted = true;  // form is submitted
      this.loading = true;    // show the loading icon

      this.user = Object.assign({}, this.regForm.value);

      console.log('this.regForm.email = ' + this.regForm.get('email').value);
      console.log('this.user.email = ' + this.user.email);

      // make username same as the registered email address
      this.user.username = this.regForm.get('email').value;

      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('Registration successful');
      }, error => {
        this.alertify.error('Registration Error: ' + error);
      }, () => { // finally routine to auto login user after registration
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/home']);
        });
      });
    }
  }
}
