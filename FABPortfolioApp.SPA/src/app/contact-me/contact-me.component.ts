import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from '../_services/alertify.service';
import { UtilityService } from '../_services/utility.service';
import { Router } from '@angular/router';
import { UserEmail } from '../_models/userEmail';

@Component({
  selector: 'app-contact-me',
  templateUrl: './contact-me.component.html',
  styleUrls: ['./contact-me.component.css']
})
export class ContactMeComponent implements OnInit {

  emailForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  userEmail: UserEmail;


  constructor(
      private formBuilder: FormBuilder,
      private http: HttpClient,
      private router: Router,

      private alertify: AlertifyService,
      private utilService: UtilityService
  ) { }

  ngOnInit() {
    // define emailForm input validators
    this.emailForm = this.formBuilder.group({
      userEmail:  ['', [ Validators.required, Validators.email ]],
      subject:    ['', [ Validators.required, Validators.minLength(2)]],
      message:    ['', [ Validators.required, Validators.minLength(2)]],
    });
  }

  sendEmail(){
    this.alertify.confirm( 'Are you sure you want to send this message?', () => {
        console.log('onSubmit -> sendEmail()->' + JSON.stringify(this.emailForm.value));
        if (this.emailForm.valid) {
            // show the loading icon
            this.loading = true;
            // assign form field values to this.folio
            this.userEmail = Object.assign({}, this.emailForm.value);
            console.log('this.userEmail->' + JSON.stringify(this.userEmail));

            // navigate to home page
            this.router.navigate(['/home']).then( () => {
                this.alertify.message('Sending email...');
                // then execute sendEmail routine
                this.utilService.sendEmail(this.userEmail).subscribe( () => {
                  this.submitted = true;  // form is submitted
                  this.alertify.success('Email successfully sent.');
                },  error => {
                    this.alertify.error('Error while sending email: ' + error);
                    this.loading = false;
                    this.router.navigate(['/home']);
                },  () => {
                    // finally
                    // once saved, stop showing busy loading animation icon
                    this.loading = false;
                });
            });


        }
    });

  }
}
