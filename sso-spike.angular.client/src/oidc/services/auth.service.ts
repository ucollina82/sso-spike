import { Injectable } from '@angular/core';
import {User, UserManager, UserManagerSettings, WebStorageStateStore } from 'oidc-client';
import { Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { CookieManagerService } from './cookie-manager.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _userManager: UserManager;
  // @ts-ignore
  private _user: User;
  private _loginChangedSubject = new Subject<boolean>();
  public loginChanged = this._loginChangedSubject.asObservable();

  // TODO 2: UC da rendere configurabile per i diversi ambienti
  private get idpSettings() : UserManagerSettings {
    return {
      authority: environment.oidcConfiguration.authority,
      client_id: environment.oidcConfiguration.client_id,
      redirect_uri: environment.oidcConfiguration.redirect_uri,
      scope: environment.oidcConfiguration.scope,
      response_type: environment.oidcConfiguration.response_type,
      //post_logout_redirect_uri: `http://localhost:4200/#/silent-callback`,
      post_logout_redirect_uri : environment.oidcConfiguration.post_logout_redirect_uri,
      userStore: new WebStorageStateStore({
        store: this.cookieStorage
      })
      // store information about Authentication in localStorage
    }
  }

  constructor(private cookieStorage: CookieManagerService) {
    this._userManager = new UserManager(this.idpSettings);
    this._userManager.events.addUserSignedOut(() => {
      console.log('Utente disconnesso. Nuova login in corso..')
      this.login();
    })
    this._userManager.events.addAccessTokenExpired(() => {
      console.log('Token scaduto. Nuova login in corso..')
      this.login();
    })
  }

  public login = () => {
    console.log("effettuo la login");
    return this._userManager.signinRedirect();
  }

  public isAuthenticated = (): Promise<boolean> => {
    return this._userManager.getUser()
      .then(user => {
        if(this._user !== user){
          // @ts-ignore
          this._loginChangedSubject.next(this.checkUser(user));
        }
        // @ts-ignore
        this._user = user;
        // @ts-ignore
        return this.checkUser(user);
      })
  }

  public finishLogin = (): Promise<User> => {
    console.log("sto per terminare la login")

    return this._userManager.signinRedirectCallback()
      .then(user => {
        console.log("termino la login")
        this._user = user;
        this._loginChangedSubject.next(this.checkUser(user));
        return user;
      })
  }

  public logout = () => {
    this._userManager.signoutRedirect();
  }

  public finishLogout = () => {
    // @ts-ignore
    this._user = null;
    return this._userManager.signoutRedirectCallback();
  }

  public getAccessToken = (): Promise<string> => {
    // @ts-ignore
    return this._userManager.getUser()
      .then(user => {
        return !!user && !user.expired ? user.access_token : null;
      })
  }

  public getUserInfo = (): Promise<string> => {
    // @ts-ignore
    return this._userManager.getUser()
      .then(user => {
        return !!user && !user.expired ? user.profile.name : null;
      })
  }

  private checkUser = (user : User): boolean => {
    return !!user && !user.expired;
  }
}


