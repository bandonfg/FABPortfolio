import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PortfolioFile } from '../../_models/portfolioFile';
import { environment } from '../../../environments/environment';
import { Portfolio } from '../../_models/portfolio';

import { PortfolioService } from '../../_services/portfolio.service';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-portfolio-detail',
  templateUrl: './portfolio-detail.component.html',
  styleUrls: ['./portfolio-detail.component.css']
})
export class PortfolioDetailComponent implements OnInit {

  folioPicUrl: string = environment.rootUrl + 'assets/images';
  portfolio: any;
  portfolioIdToQuery: number;
  isFirstCarouselPic: boolean;

  loggedUser: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertify: AlertifyService,
    private portfolioService: PortfolioService
  ) { }

  ngOnInit() {

    this.isFirstCarouselPic = false;

    // get portfolio-detail route params from portfolio-list
    this.route
      .queryParams
      .subscribe(params => {
        this.portfolioIdToQuery =  params['id'] ;
      });

    this.getPortfolioById(this.portfolioIdToQuery);
  }

  getPortfolioById(id: number) {
    this.portfolioService.getPortfoliosById(id)
      .subscribe( (res: Portfolio[]) => {
        this.portfolio = res;
        console.log('portfolios loaded.');
      }, error => {
        console.log('portfolio load error: ' + error);
      });
  }

  isFirstPicture() {
    if ( this.isFirstCarouselPic === false) {
      this.isFirstCarouselPic = true;
      return true;
    }
    return false;
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    this.loggedUser = JSON.parse(localStorage.getItem('user'));
    return !!token;
  }


  // DELETE api/portfolio/{srcTable}/{id}
  // Params {srcTable} refer to 1 = Portfolio, 2 = PortfolioFile table
  //        {id} can refer to Portfolio.Id or PortfolioPicture.Id
  deletePortfolio(id: number) {
    this.alertify.confirm( 'Are you sure you want to delete this portfolio?', () => {
      this.portfolioService.deletePortfolio(1, id).subscribe(() => {
        this.alertify.success('Portfolio has been deleted');
        // refresh portfolio list
        this.router.navigate(['/portfolio']);
      }, error => {
        this.alertify.error('Delete portfolio error: ' + error);
      });
    });
  }

}
