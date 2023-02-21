import { Injectable } from '@angular/core';
import {
    HttpEvent,
    HttpInterceptor,
    HttpHandler,
    HttpRequest,
    HttpErrorResponse,
    HttpHeaders
} from '@angular/common/http';
import {from, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';

import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(
      private readonly _auth2Service: AuthService,
        private readonly _router: Router
    ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      // @ts-ignore
    return from(
        this._auth2Service.getAccessToken()
          .then(token => {
            const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
            const authRequest = req.clone({ headers });
            return next.handle(authRequest)
              .pipe(
                tap(
                  () => {},
                  error => {
                    const respError = error as HttpErrorResponse;
                    if (
                      respError &&
                      (respError.status === 401 ||
                        respError.status === 403)
                    ) {
                      //debugger;
                      console.log("401 rieffettuo la login")
                      this._auth2Service.login();
                    }
                  }
                )
              )
              .toPromise();
          })
      )
  }


}
