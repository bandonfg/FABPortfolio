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

  // GET api/portfolio - get all portfolio and picture(s) by id
  getPortfoliosById(id: number) {
    return this.http.get<Portfolio[]>(this.baseUrl + '/' + id);
  }

  // GET api/portfolio/company - get unique list of portfolio companies
  getUniquePortfolioCompanies() {
    return this.http.get<string[]>(this.baseUrl + '/company');
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

  // GET api/portfolio/file - get all picture(s) by portfolio id
  getPortfolioFilesById(id: number) {
    return this.http.get<PortfolioFile[]>(this.baseUrl + '/file/' + id);
  }

  // POST api/portfolio/fileinfo
  // POST api/portfolio/picture (previous)
  addPortfolioPicture(folioPic: PortfolioFile) {
    // return this.http.post(this.baseUrl + '/picture', folioPic);
    return this.http.post(this.baseUrl + '/fileinfo', folioPic);
  }

  // DELETE api/portfolio/picture/id
  deletePortfolioPicture(id: number) {
    console.log('portfolioService->deletePortfolioPicture(id)');
    return this.http.delete(this.baseUrl + '/picture/' + id);
    
  }

}
