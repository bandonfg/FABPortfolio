
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEvent} from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserEmail } from '../_models/userEmail';
import { environment } from '../../environments/environment';
import { Log } from '../_models/log';

@Injectable()
export class UtilityService {

  // return /api
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // POST api/portfolio/file
  // file upload utility service
  // file from event.target.files[0]
  uploadFile(url: string, file: File): Observable<HttpEvent<any>> {

    const formData = new FormData();
    formData.append('upload', file);

    console.log('util.service->uploadFile()->' + JSON.stringify(file.name) );

    const params = new HttpParams();
    const headers = new HttpHeaders({ 'Authorization': 'Bearer ' + localStorage.getItem('token'), });
    const options = { headers: headers, params: params, reportProgress: true, };
    const req = new HttpRequest('POST', url, formData, options);
    return this.http.request(req);
  }

  sendEmail( userEmail: UserEmail  ) {
    return this.http.post(this.baseUrl + '/user/email', userEmail);
  }

  ////////////////////////////////////////////////////////////////////
  // Visitor Log Service                                            //
  ////////////////////////////////////////////////////////////////////

  // GET          api/log
  // Description  get visitor log list. Paged info passed as header params
  getLogs(pageNumber: any, pageSize: any) {
      const headers = new HttpHeaders();
      headers.append('Content-Type', 'application/json');
      const params = new HttpParams()
          .set('pageNumber', pageNumber)
          .set('pageSize', pageSize);

      return this.http.get<Log[]>(this.baseUrl + '/log', {headers: headers, params: params} );
  }

  // GET          api/log/{id}
  // Description  get log by id
  getLogById(id: number) {
    return this.http.get<Log[]>(this.baseUrl + '/log' + id);
  }


  // DELETE       api/log/{id}
  // Description  delete log by id
  deleteLog(id: number) {
    return this.http.delete(this.baseUrl + '/log/' + id);
  }


  // POST         api/log
  // Description  create or add visitor log. Data will be provided by GeoLite db.
  createLog() {
    return this.http.post(this.baseUrl + '/log', '');
  }
  ////////////////////////////////////////////////////////////////////
  // End of Visitor Log Service                                     //
  ////////////////////////////////////////////////////////////////////


}

