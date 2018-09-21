import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';

import { Portfolio } from '../_models/portfolio';
import { PortfolioFile } from '../_models/portfolioFile';
import { PaginatedResult } from '../_models/pagination';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

// strange error when providedIn: 'root' is removed
@Injectable({
  providedIn: 'root'
})
export class PortfolioService {

  baseUrl = environment.apiUrl + '/portfolio';

  constructor(private http: HttpClient ) { }

  /////////////////////////////////
  /// Portfolio Service Methods ///
  /////////////////////////////////

  // GET api/portfolio - get all portfolios
  getPortfolios() {
    return this.http.get<Portfolio[]>(this.baseUrl);
  }

  /* paginated code with error
  getPortfolios(page?, itemsPerPage?, portfolioParams?): Observable<PaginatedResult<Portfolio[]>> {
    const paginatedResult: PaginatedResult<Portfolio[]> = new PaginatedResult<Portfolio[]>();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }
    return this.http.get<Portfolio[]>(this.baseUrl,
      {observe: 'response', params})
      .pipe(
         map(response => {
           paginatedResult.results = response.body;
           if (response.headers.get('Pagination') != null) {
              paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
           }
           return paginatedResult;
         })
      );
  }
  */

  // GET api/portfolio - get all portfolio and picture(s) by id
  getPortfoliosById(id: number) {
    return this.http.get<Portfolio[]>(this.baseUrl + '/' + id);
  }

  // DELETE   api/portfolio
  // deletePortfolio(srcTable: number, id: number)
  // Params   srcTable can be Portfolio(1) or PortfolioPicture (2)
  //          id can be Portfolio.Id or PortfolioPicture.Id
  deletePortfolio(srcTable: number, id: number) {
    return this.http.delete(this.baseUrl + '/' + srcTable + '/' + id);
  }

  // POST api/portfolio
  createPortfolio(folio: Portfolio) {
    return this.http.post(this.baseUrl, folio);
  }

  // PUT api/portfolio
  updatePortfolio(id: number, folio: Portfolio) {
    return this.http.put(this.baseUrl + '/edit/' + id, folio);
  }


  /////////////////////////////////////////
  /// Portfolio Picture Service Methods ///
  /////////////////////////////////////////
  // POST api/portfolio/picture
  addPortfolioPicture(folioPic: PortfolioFile) {
    return this.http.post(this.baseUrl + '/picture', folioPic);
  }

  // DELETE api/portfolio/picture/id
  deletePortfolioPicture(id: number) {
    console.log('portfolioService->deletePortfolioPicture(id)');
    return this.http.delete(this.baseUrl + '/picture/' + id);
  }

}
