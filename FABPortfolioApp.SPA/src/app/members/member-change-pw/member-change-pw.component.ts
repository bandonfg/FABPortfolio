import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from '../../_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { UserPW } from '../../_models/user-pw';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-member-change-pw',
  templateUrl: './member-change-pw.component.html',
  styleUrls: ['./member-change-pw.component.css']
})
export class MemberChangePWComponent implements OnInit {

  userPWForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';
  userToken = JSON.parse(localStorage.getItem('user'));


  userPW: any = {}; // UserPW;

  constructor(
      private formBuilder: FormBuilder,
      private http: HttpClient,
      private route: ActivatedRoute,
      private router: Router,

      private authService: AuthService,
      private alertify: AlertifyService
  ) { }

  ngOnInit() {

    // define userPWForm input validators
    this.userPWForm = this.formBuilder.group({
      username:         [this.userToken.username, [ Validators.required, Validators.email ]],
      currentPassword:  ['', [ Validators.required, Validators.minLength(2)]],
      newPassword:      ['', [Validators.required, Validators.minLength(6), Validators.maxLength(20) ]],
      confirmPassword:  ['', [ Validators.required, Validators.minLength(6), Validators.maxLength(20) ]]
    }, {
        validator: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

  updateUserPassword() {
    this.alertify.confirm( 'Are you sure you want to update your password?', () => {

        if (this.userPWForm.valid) {
            // show loading icon
            this.loading = true;
            // assign form field values to this.userPW
            this.userPW = Object.assign({}, this.userPWForm.value);

            this.authService.updatePassword(this.userPW).subscribe( (res: any) => {
                this.submitted = true;  // form submitted, prevent edit to form
                this.loading = false;
            },  error => {
                this.submitted = false;  // allow edit to form
                this.alertify.error('Password update failed! Please re-enter password and try again.');
                this.loading = false;
            }, () => {
              // once saved, stop showing busy loading animation icon
              this.loading = false;
              this.alertify.success('Your password has been successfully updated.');
              this.router.navigate(['/home']);
        });


          } // end of if (this.userPWForm.valid)
    }); // end of this.alertify.confirm()
  } // end of updateUserPassword()

}
