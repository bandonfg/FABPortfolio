import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

import { AlertifyService } from '../_services/alertify.service';

import { Log } from '../_models/log';
import { LogService } from '../_services/log.service';

@Component({
  selector: 'app-log',
  templateUrl: './log.component.html',
  styleUrls: ['./log.component.css']
})
export class LogComponent implements OnInit {

  loggedUser: any;

  loading = false;
  error = '';

  public totalItems: number;
  public pageSize: number;
  public currentPage: number;
  public totalPages: number;
  pages: any[] = [];

  public pageLoading = true;
  public companyFilter = '';

  logs: Log[] = [];

  constructor(
      private http: HttpClient,
      private router: Router,
      private alertify: AlertifyService,
      private logService: LogService
  ) { }

  ngOnInit() {

    // init log paging params
    this.pageSize = 10;
    this.currentPage = 1;

    this.getLogs(this.currentPage, this.pageSize);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    this.loggedUser = JSON.parse(localStorage.getItem('user'));
    return !!token;
  }

  setToPage(pageNumber: number): void {
    // display page loading spinner while retrieving data
    this.pageLoading = true;
    this.getLogs(pageNumber, this.pageSize);
  }

  getLogs(pageNumber: number, pageSize: number) {

    // console.log('log.component.ts->before->getLogs()->execution');

    this.logService.getLogs(pageNumber, pageSize).subscribe( (res: any) => {
        // console.log('log.component.ts->getLogs()->subscribe->res' + JSON.stringify(res));
        // stop page loading spinner animation
        this.pageLoading = false;

        // get logs data
        this.logs = res.logs;

        // console.log('this.logs->' + this.logs);

        // get paging info
        this.totalItems = res.totalCount;
        this.pageSize = pageSize;
        this.currentPage = pageNumber;
        this.totalPages = res.totalPages;

        let logPageCtr = 1;

        // calculate pages
        this.pages = [];
        for (logPageCtr = 0; logPageCtr < Math.ceil(this.totalItems / pageSize); logPageCtr++) {
            this.pages[logPageCtr] = logPageCtr + 1;
        }
      console.log('logs loaded.');

    }, error => {
      // stop page loading spinner animation
      this.pageLoading = false;
      console.log('logs load error: ' + error);
    });
  }

  deleteLog(id: number) {
    this.alertify.confirm( 'Are you sure you want to delete this log?', () => {
      this.logService.deleteLog(id).subscribe(() => {
        this.alertify.success('Log has been deleted');
        // refresh portfolio list
        // this.loadAllPortfolios();
        this.getLogs(this.currentPage, this.pageSize);
      }, error => {
        this.alertify.error('Delete log error: ' + error);
      });
    });
  }
}
