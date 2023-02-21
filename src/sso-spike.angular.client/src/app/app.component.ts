import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/oidc/services/auth.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'sso-spike.angular.client';
  userAuthenticated: boolean =false;

  constructor(private _authService: AuthService, private http: HttpClient) {
    this._authService.loginChanged
      .subscribe(userAuthenticated => {
        console.log("login changed" + userAuthenticated)
        this.userAuthenticated = userAuthenticated;
      })
  }

  ngOnInit(): void {
    const urlRequest = `${environment.apiHostUrl}/WeatherForecast`;

    const headers = this.createHeaders();
    const options = {
      headers,
      withCredentials: false
    };
    this.http.get(urlRequest, options).pipe(
      map((response) => {
      })).subscribe();
  }

  private createHeaders(): HttpHeaders {
    const headers = new HttpHeaders({
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Headers': 'Content-Type, Origin , Access-Control-* , X-Requested-With, Accept, Content-Disposition',
      'Content-Type': 'application/json,charset=utf-8,text/csv',
      Accept: 'application/json',
      Allow: 'GET, POST, PUT, DELETE, OPTIONS, HEAD'
    });
    return headers;
  }

  logout() {
    this._authService.logout();
  }
}
