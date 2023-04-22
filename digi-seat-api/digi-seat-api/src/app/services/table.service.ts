import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable } from 'rxjs';

@Injectable()
export class TableService {

  constructor(
    private http: HttpClient,
    private config: ConfigService) { }

  get(partySize:string): Observable<object> {
    let url = `${this.config.apiUrl()}tables/?partySize=${partySize}`
    let result = this.http.get(url);
    console.log(result);
    return result;
  }

}
