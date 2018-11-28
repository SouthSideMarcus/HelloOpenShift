import { Component, Inject } from '@angular/core';
//import { FormsModule } from '@angular/forms';
import { SsotestService, TestResults } from './ssotest.service';

@Component({
    selector: 'app-ssotest',
    templateUrl: './ssotest.component.html',
    styleUrls: ['./ssotest.component.css']
})
/** ssotest component*/
export class SsotestComponent {

  public SSOToken: string = '';
  public SSOUser: string = '';

  private _ssoService: SsotestService;

  /** ssotest ctor */
  constructor(ssoService: SsotestService) {
    this._ssoService = ssoService;
  }

  fetchSSOTokey() {

    try {

      this.SSOToken = "Calling server...";

      this._ssoService.fetchSSOTokenCookie().subscribe((data: TestResults) => {
        console.log('got success back from ssotestservice call');
        if (data.is_success)
          this.SSOToken = data.sso_token;
        else
          this.SSOToken = data.error_msg;
      }, error => this.handleError(error));

      // testing getting the full response
      //this._ssoService.fetchSSOTokenCookieResponse().subscribe(resp => {
      //  console.log('got success back from ssotestservice call');

      //  const keys = resp.headers.keys();
      //  var headers = keys.map(key =>
      //    `${key}: ${resp.headers.get(key)}`);

      //  let data: TestResults = resp.body;
      //  if (data.is_success)
      //    this.SSOToken = data.sso_token;
      //  else
      //    this.SSOToken = data.error_msg;
      //}, error => console.error(error));

    }
    catch (err) {
      this.handleError(err);
    }


  }

  callSSOService() {
    try {

      this.SSOUser = "Calling server...";

      this._ssoService.callSSOService(this.SSOToken, 'Prod').subscribe((data: TestResults) => {
        console.log('got success back from callSSOService call');
        if (data.is_success)
          this.SSOUser = data.user_data;
        else
          this.SSOUser = data.error_msg;
      }, error => this.handleError(error));

      // testing getting the full response
      //this._ssoService.fetchSSOTokenCookieResponse().subscribe(resp => {
      //  console.log('got success back from ssotestservice call');

      //  const keys = resp.headers.keys();
      //  var headers = keys.map(key =>
      //    `${key}: ${resp.headers.get(key)}`);

      //  let data: TestResults = resp.body;
      //  if (data.is_success)
      //    this.SSOToken = data.sso_token;
      //  else
      //    this.SSOToken = data.error_msg;
      //}, error => console.error(error));

    }
    catch (err) {
      this.handleError(err);
    }

  }

  private handleError(err: any) {
    this.SSOUser = err;
    console.error(err);

  }

}
