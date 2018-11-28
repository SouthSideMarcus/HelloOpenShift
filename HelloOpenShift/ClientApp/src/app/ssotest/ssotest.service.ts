import { Component, Inject, Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { ErrorObservable } from 'rxjs/observable/ErrorObservable';
//import { Observable, throwError } from 'rxjs/';
import { catchError, retry } from 'rxjs/operators';

export interface TestResults {
  is_success: boolean;
  error_msg: string;
  sso_token: string;
  user_data: string;
}

export class SearchTokenInput {
  environment: string;
  portal: string;
  sso_token: string;
}

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    'Authorization': 'my-auth-token'
  })
};

@Injectable()
export class SsotestService {

  private _http: HttpClient;
  private _baseURL: string;


  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._http = http;
    this._baseURL = baseUrl;

  }

  fetchSSOTokenCookie(): Observable<TestResults> {

    console.log('SsotestService::fetchSSOTokenCookie start');

    return this._http.get<TestResults>(this._baseURL + 'api/SampleData/GetSSOToken')
        .pipe(retry(3), catchError(this.handleError));
  }

  fetchSSOTokenCookieResponse(): Observable<HttpResponse<TestResults>> {

    return this._http.get<TestResults>(
        this._baseURL + 'api/SampleData/GetSSOToken', { observe: 'response' })
        .pipe(retry(3), catchError(this.handleError));

  }

  callSSOService(ssoToken: string, enviro: string): Observable<TestResults> {

    console.log('SsotestService::fetchSSOTokenCookie start');

    let p: SearchTokenInput = new SearchTokenInput();
    p.sso_token = ssoToken;
    p.environment = enviro;

    return this._http.post<TestResults>(this._baseURL + 'api/SampleData/CallSSOServiceAsync', p, httpOptions)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    let msg: string = 'Error calling server. Details: ';
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      msg += error.error.message;
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      msg += `Backend returned code ${error.status}, ` + `body was: ${error.error}`;
    }
    // return an observable with a user-facing error message
    console.error(msg);
    // Observable.throw(msg);
    return new ErrorObservable(msg);
  };

}
