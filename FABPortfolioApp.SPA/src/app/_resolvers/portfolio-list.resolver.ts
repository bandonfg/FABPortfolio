import {Injectable} from '@angular/core';
import {User} from '../_models/user';
import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { PortfolioService } from '../_services/portfolio.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Portfolio } from '../_models/portfolio';

@Injectable()
export class PortfolioListResolver implements Resolve<Portfolio[]> {
    pageNumber = 1;
    pageSize = 2;
    companyFilter = '';

    constructor(
        private portfolioService: PortfolioService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Portfolio[]> {
        return this.portfolioService.getPortfolios(this.pageNumber, this.pageSize, this.companyFilter).pipe(
        // return this.portfolioService.getPortfolios().pipe(
            catchError(error => {
                this.alertify.error('PortfolioListResolver: Problem retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }

}
