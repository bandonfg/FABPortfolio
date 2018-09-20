
import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEvent} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class UtilityService {

  constructor(private http: HttpClient) { }

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
}

