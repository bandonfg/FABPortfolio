import {Injectable} from '@angular/core';
import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { LogService } from '../_services/log.service';
import { Log } from '../_models/log';

@Injectable()
export class LogListResolver implements Resolve<Log[]> {
    pageNumber = 1;
    pageSize = 10;

    constructor(
        private logService: LogService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Log[]> {
        return this.logService.getLogs(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('LogListResolver: Problem retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
