import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { first } from 'rxjs/operators';

// import { PaginationModule } from 'ngx-bootstrap';

import { environment } from '../../../environments/environment';

import { Portfolio } from '../../_models/portfolio';
import { PortfolioService } from '../../_services/portfolio.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Pagination, PaginatedResult } from '../../_models/pagination';

@Component({
  selector:     'app-portfolio',
  templateUrl:  './portfolio-list.component.html',
  styleUrls: [  './portfolio-list.component.css']
})

export class PortfolioListComponent implements OnInit {

  folioPicUrl: string = environment.rootUrl + '/images';

  loggedUser: any;

  portfolioParams: any = {};
  public portfolios: Portfolio[] = [];
  pagination: Pagination;


  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private portfolioService: PortfolioService,
    private alertify: AlertifyService
    ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.portfolios = data['portfolios'].result;
      this.pagination = data['portfolios'].pagination;
    });

    // this.loadAllPortfolios();
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    this.loggedUser = JSON.parse(localStorage.getItem('user'));
    return !!token;
  }


  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadAllPortfolios();
  }

  loadAllPortfolios() {
    this.portfolioService.getPortfolios(this.pagination.currentPage, this.pagination.itemPerPage, this.portfolioParams)
    .subscribe( (res: PaginatedResult<Portfolio[]>) => {
      this.portfolios = res.results;
      this.pagination = res.pagination;
      console.log('portfolios loaded.');
    }, error => {
      console.log('portfolio load error: ' + error);
    });
  }

  // DELETE api/portfolio/{srcTable}/{id}
  // Params {srcTable} refer to 1 = Portfolio, 2 = PortfolioTable
  //        {id} can refer to Portfolio.Id or PortfolioPicture.Id
  deletePortfolio(id: number) {
    this.alertify.confirm( 'Are you sure you want to delete this portfolio?', () => {
      this.portfolioService.deletePortfolio(1, id).subscribe(() => {
        this.alertify.success('Portfolio has been deleted');
        // refresh portfolio list
        this.loadAllPortfolios();
      }, error => {
        this.alertify.error('Delete portfolio error: ' + error);
      });
    });
  }
}
