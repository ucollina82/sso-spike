import {Injectable} from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  RouterStateSnapshot
} from '@angular/router';

import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
    private readonly _auth2Service: AuthService
  ) {
  }

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    //const isLogged = await this._auth2Service.isAuthenticated();
    //alert("islogged" + (isLogged)+(route.url));
    //if (isLogged) {
      //  return true;
      //}
    //
    //await this._authService.startAuthentication();

    return true;
  }
}
