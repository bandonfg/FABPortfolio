import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PortfolioService } from '../../_services/portfolio.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Portfolio } from '../../_models/portfolio';

import {HttpClient, HttpParams, HttpRequest, HttpEvent, HttpEventType, HttpResponse} from '@angular/common/http';
import { UtilityService } from '../../_services/utility.service';
import { environment } from '../../../environments/environment';
import { PortfolioFile } from '../../_models/portfolioFile';
import { Portfolios } from '../../_models/portfolios';

@Component({
  selector: 'app-portfolio-create',
  templateUrl: './portfolio-create.component.html',
  styleUrls: ['./portfolio-create.component.css']
})
export class PortfolioCreateComponent implements OnInit {

  folioPicUrl = environment.rootUrl + 'assets/images';
  uploadUrl = environment.apiUrl + '/portfolio/file';

  folioForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  folio: Portfolio;
  folioPicsToList: Portfolio[] = [];

  // test new
  folioFiles: PortfolioFile[] = [];

  folioPicToAdd: any = {};
  portfolioId: number;

  progress: any;

  idToEdit = 0;
  workDuration = '';
  submitModeLabel: String = 'test';


  constructor(
    private formBuilder: FormBuilder,
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,

    private portfolioService: PortfolioService,
    private alertify: AlertifyService,
    private utilService: UtilityService) { }

  ngOnInit() {
    // portfolio form input validators
    this.folioForm = this.formBuilder.group({
      project:        ['', [ Validators.required, Validators.minLength(2), Validators.maxLength(200) ]],
      description:    ['', [ Validators.required, Validators.minLength(2), Validators.maxLength(1000)]],
      from:           ['', [Validators.required]],
      to:             ['', [Validators.required]],
      company:        ['', [Validators.required]],
      location:       ['', [Validators.required]],
      url:            ['']
      // portfolioPictures:['', Validators.required]
    });

      // set form values when in edit more (idToEdit > 0)
      // idToEdit refer to Portfolio.Id
      this.route.queryParams
          .subscribe(params => {
            // Defaults to 0 (Create or New Mode) if no query param provided.
          this.idToEdit = params['idToEdit'] || 0;
          console.log('portfolio-create->idToEdit->' + this.idToEdit);

          this.folioForm.controls['project'].setValue( params['project'] || '' );
          this.folioForm.controls['description'].setValue( params['description'] || '' );
          this.folioForm.controls['from'].setValue( params['from'] || '' );
          this.folioForm.controls['to'].setValue( params['to'] || '' );
          this.folioForm.controls['company'].setValue( params['company'] || '' );
          this.folioForm.controls['location'].setValue( params['location'] || '' );
          this.folioForm.controls['url'].setValue( params['url'] || '' );
        });

    // get list of portfolio pictures when in edit mode (idToEdit > 0)
    if (this.idToEdit > 0) {
      this.getPortfolioPictures(this.idToEdit);
    }

  } // end of ngOnInit()

  get f() { return this.folioForm.controls; }

  //////////////////////////////////////////////
  // routine to add or edit portfolio entries //
  //////////////////////////////////////////////
  submitPortfolioEntries() {
    // submit entries when form is valid
    if (this.folioForm.valid) {
      // show the loading icon
      this.loading = true;
      // assign form field values to this.folio
      this.folio = Object.assign({}, this.folioForm.value);

      // Create new portfolio (this.idToEdit is 0)
      if (this.idToEdit === 0) {
          this.portfolioService.createPortfolio(this.folio).subscribe( (res: Portfolio) => {
              this.submitted = true;  // form is submitted
              this.portfolioId = res.id;
              this.alertify.success('Portfolio successfully created.');
          }, error => {
                this.alertify.error('Error while creating portfolio: ' + error);
                this.loading = false;
          }, () => {
              // once saved, stop showing busy loading animation icon
              this.loading = false;
          });
      } else if ( this.idToEdit > 0 ) { // Update portfolio (this.idToEdit is 1)
          this.folio.id = this.idToEdit;
          this.portfolioId = this.idToEdit;

          this.portfolioService.updatePortfolio(this.idToEdit, this.folio).subscribe( (res: Portfolio) => {
              this.submitted = true;  // form is submitted
              this.alertify.success('Portfolio successfully created.');
          }, error => {
              console.log('else if this.idToEdit > 1 block -> error: ' + error);
              this.alertify.error('Error while updating portfolio: ' + error);
              this.loading = false;
          }, () => {
              // once saved, stop showing busy loading animation icon
              this.loading = false;
          });
      }
    }
  }
  /////////////////////////////////////////////////////
  // end of routine to add or edit portfolio entries //
  /////////////////////////////////////////////////////


  // portfolio photo upload routines
  uploadPhoto(files: FileList) {
    // determine if there is file selected for upload
    if (files.length === 0) {
      console.log('No file selected!');
      return;
    }
    // get file info
    const file: File = files[0];

    /* retained here for debugging only
    console.log('uploadPhoto(files)->Length of array ->' + JSON.stringify(files.length));
    console.log('uploadPhoto(files)->' + file.name);
    console.log('this.portfolioId -> (new) ' + this.portfolioId);
    console.log('this.idToEdit -> (edit) ' + this.idToEdit);
    */

    // file upload service
    this.utilService.uploadFile(this.uploadUrl + '/' + ( this.portfolioId || this.idToEdit ) , file)
      .subscribe(
        event => {
          if (event.type === HttpEventType.UploadProgress) {
            this.progress = Math.round(100 * event.loaded / event.total);
            console.log(`File is ${this.progress}% loaded.`);
          } else if (event instanceof HttpResponse) {
            console.log('File is completely loaded!');
          }
        },
        (err) => {
          console.log('Upload Error:', err);
          this.alertify.error('Upload Error: ' + err);
        }, () => {
          console.log('Upload done');
          this.alertify.success('File successfully uploaded.');

         this.getPortfolioPictures( ( this.portfolioId || this.idToEdit ) );
        }
      );

  } // end of uploadPhoto

  // GET api/portfolio/{id} - Get list of pictures by portfolio id
  // Param id refers to Portfolio.Id
  getPortfolioPictures(id: number) {
    // this.portfolioService.getPortfoliosById(id).subscribe( (p: Portfolio[]) => {
      // this.folioPicsToList = p;
    console.log('getPortfolioPictures(id)->');
    this.portfolioService.getPortfolioFilesById(id).subscribe( (p: PortfolioFile[]) => {
        console.log('this.portfolioService.getPortfolioFilesById(id).subscribe(p)->' + JSON.stringify(p) );
        this.folioFiles = p;
    }, error => {
        console.log('getPortfolioPictures(id) -> ' + id);
        this.alertify.error('Error while listing portfolio picture(s): ' + error);
        this.loading = false;
    });
  }

  // DELETE api/portfolio/{srcTable}/{id}
  // Params {srcTable} refers to Portfolio(1) or PortfolioPicture(2) source table
  //        {id} is  PortfolioPicture.Id
  deletePortfolioPicture(id: number) {
    this.alertify.confirm( 'Are you sure you want to delete this portfolio picture?', () => {
      console.log();
      this.portfolioService.deletePortfolio(2, id).subscribe(() => {
        this.alertify.success('Portfolio has been deleted');
        // refresh portfolio list
        this.getPortfolioPictures(this.portfolioId || this.idToEdit);
      }, error => {
        this.alertify.error('Delete portfolio error: ' + error);
      });
    });
  }

  ////////////////////////////
  /// file upload routines ///
  ////////////////////////////
    // At the drag drop area
  // (drop)='onDropFile($event)'
  onDropFile(event: DragEvent) {
    event.preventDefault();
    this.uploadPhoto(event.dataTransfer.files);
  }

  // At the drag drop area
  // (dragover)='onDragOverFile($event)'
  onDragOverFile(event) {
    event.stopPropagation();
    event.preventDefault();
  }

  // At the file input element
  // (change)='selectFile($event)'
  selectFile(event) {
    this.uploadPhoto(event.target.files);
  }
  ///////////////////////////////////
  /// end of file upload routines ///
  ///////////////////////////////////

} // end of PortfolioCreateEditComponent

