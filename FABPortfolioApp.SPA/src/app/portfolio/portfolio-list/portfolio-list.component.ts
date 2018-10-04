import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router, Routes } from '@angular/router';
import { first } from 'rxjs/operators';

// import { PaginationModule } from 'ngx-bootstrap';

import { environment } from '../../../environments/environment';

import { Portfolio } from '../../_models/portfolio';
import { PortfolioService } from '../../_services/portfolio.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Pagination, PaginatedResult } from '../../_models/pagination';
import { PortfolioFile } from '../../_models/portfolioFile';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector:     'app-portfolio',
  templateUrl:  './portfolio-list.component.html',
  styleUrls: [  './portfolio-list.component.css']
})

export class PortfolioListComponent implements OnInit {

  folioPicUrl: string = environment.rootUrl + 'assets/images';

  loggedUser: any;

  portfolioParams: any = {};
  portfolios: Portfolio[] = [];
  
  // for deletion neils code pagination: Pagination;

  // list of companies used by filter dropdown list
  companies: string[] = [];

  public totalItems: number;
  public pageSize: number;
  public currentPage: number;
  public totalPages: number;
  pages: any[] = [];

  public pageLoading = true;
  public companyFilter = '';


  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private authService: AuthService,
    private portfolioService: PortfolioService,
    private alertify: AlertifyService
    ) {}

  ngOnInit() {
    /* pagination related code
    this.route.data.subscribe(data => {
      this.portfolios = data['portfolios'].result;
      this.pagination = data['portfolios'].pagination;
    });
    */
    this.getUniquePortfolioCompanies();

    this.pageSize = 8;
    this.currentPage = 1;
    this.companyFilter = '';

    this.loadAllPortfolios(this.currentPage, this.pageSize);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    this.loggedUser = JSON.parse(localStorage.getItem('user'));
    return !!token;
  }

  getUniquePortfolioCompanies() {
    this.portfolioService.getUniquePortfolioCompanies()
    .subscribe( (res: string[]) => {
      this.companies = res;
      console.log('unique companies loaded.');
    }, error => {
      console.log('unique companies load error: ' + error);
    });
  }

  filterByCompany(company: string) {
    this.companyFilter = company;
    this.loadAllPortfolios(this.currentPage, this.pageSize);
  }

  setToPage(pageNumber: number): void {
    // display page loading spinner while retrieving data
    this.pageLoading = true;
    this.loadAllPortfolios(pageNumber, this.pageSize);
  }

  loadAllPortfolios(pageNumber: number, pageSize: number) {
    this.portfolioService.getPortfolios(pageNumber, pageSize, this.companyFilter)
    .subscribe( (res: any) => {

        // stop page loading spinner animation
        this.pageLoading = false;

        console.log('Paging->portfolio-list->loadAllPortfolios()->res' + JSON.stringify(res));

        // get the portfolios data
        this.portfolios = res.portfolios;

        console.log('this.portfolios->' + this.portfolios);
        // get paging info
        this.totalItems = res.totalCount;
        // this.pageSize = pageSize;
        this.currentPage = pageNumber;
        this.totalPages = res.totalPages;

        let portfolioPageCtr = 1;

        // calculate pages
        this.pages = [];
        for (portfolioPageCtr = 0; portfolioPageCtr < Math.ceil(this.totalItems / pageSize); portfolioPageCtr++) {
            this.pages[portfolioPageCtr] = portfolioPageCtr + 1;
        }
      console.log('portfolios loaded.');

    }, error => {
      // stop page loading spinner animation
      this.pageLoading = false;
      console.log('portfolio load error: ' + error);
    });

  }

  // DELETE api/portfolio/{srcTable}/{id}
  // Params {srcTable} refer to 1 = Portfolio, 2 = PortfolioFile table
  //        {id} can refer to Portfolio.Id or PortfolioPicture.Id
  deletePortfolio(id: number) {
    this.alertify.confirm( 'Are you sure you want to delete this portfolio?', () => {
      this.portfolioService.deletePortfolio(1, id).subscribe(() => {
        this.alertify.success('Portfolio has been deleted');
        // refresh portfolio list
        // this.loadAllPortfolios();
        this.loadAllPortfolios(this.currentPage, this.pageSize);
      }, error => {
        this.alertify.error('Delete portfolio error: ' + error);
      });
    });
  }

  // logout() {
  //    this.authService.logout();
  // }

}
