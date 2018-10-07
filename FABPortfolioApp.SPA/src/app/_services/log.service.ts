
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEvent} from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserEmail } from '../_models/userEmail';
import { environment } from '../../environments/environment';
import { Log } from '../_models/log';

@Injectable({
  providedIn: 'root'
})

export class LogService {

  // return /api
  baseUrl = environment.apiUrl + '/geolog';

  constructor(
    private http: HttpClient) {

    console.log('LogService->constructor()->baseUrl->' + this.baseUrl);
  }

  ////////////////////////////////////////////////////////////////////
  // Visitor Log Service                                            //
  ////////////////////////////////////////////////////////////////////

  // GET          api/log
  // Description  get visitor log list. Paged info passed as header params
  getLogs(pageNumber: number, pageSize: number) {

      console.log('log.service.ts->getLogs()->');
      console.log('log.service.ts->getLogs()->this.baseUrl->' + this.baseUrl);
      console.log('log.service.ts->getLogs()->pageNumber ->' + pageNumber);
      console.log('log.service.ts->getLogs()->pageSize ->' + pageSize);

      const headers = new HttpHeaders();
      headers.append('Content-Type', 'application/json');
      // const headers = new HttpHeaders({ 'Authorization': 'Bearer ' + localStorage.getItem('token'), });
      const params = new HttpParams()
          .set('pageNumber', pageNumber.toString())
          .set('pageSize', pageSize.toString());

      return this.http.get<any[]>(this.baseUrl, {headers: headers, params: params} );
  }

  // GET          api/log/{id}
  // Description  get log by id
  getLogById(id: number) {
      return this.http.get<Log[]>(this.baseUrl + '/' + id);
  }


  // DELETE       api/log/{id}
  // Description  delete log by id
  deleteLog(id: number) {
      return this.http.delete(this.baseUrl + '/' + id);
  }


  // POST         api/log
  // Description  create or add visitor log. Location Data will be provided by GeoLite db.
  createLog() {
      // error occurs when passing not passing value(s) hence the empty string
      return this.http.post(this.baseUrl, '');
  }
  ////////////////////////////////////////////////////////////////////
  // End of Visitor Log Service                                     //
  ////////////////////////////////////////////////////////////////////
}

