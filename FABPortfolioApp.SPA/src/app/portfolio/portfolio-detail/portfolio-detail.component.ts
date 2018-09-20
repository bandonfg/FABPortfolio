import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PortfolioFile } from '../../_models/portfolioFile';
import { environment } from '../../../environments/environment';
import { Portfolio } from '../../_models/portfolio';

import { PortfolioService } from '../../_services/portfolio.service';

@Component({
  selector: 'app-portfolio-detail',
  templateUrl: './portfolio-detail.component.html',
  styleUrls: ['./portfolio-detail.component.css']
})
export class PortfolioDetailComponent implements OnInit {

  folioPicUrl: string = environment.rootUrl + '/images';
  portfolio: any;
  portfolioIdToQuery: number;
  isFirstCarouselPic: boolean;

  constructor(
      private route: ActivatedRoute,
      private portfolioService: PortfolioService) { }

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


}
