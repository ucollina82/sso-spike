import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import {AuthUrlConstantServiceToken, IAuthUrlConstantService } from '../services';
import { AuthService } from '../services/auth.service';


@Component({
    selector: 'signin-callback',
    templateUrl: './login-callback.component.html',
})
export class LoginCallbackComponent implements OnInit {
    constructor(
        //@Inject(AuthUrlConstantServiceToken)
        //private readonly _authUrlConstantService: IAuthUrlConstantService,

        private readonly _auth2Service: AuthService,
        private readonly _router: Router
    ) {  }


   async ngOnInit() {
    console.log("signin-callback chiamata");
    await this._auth2Service.finishLogin();
    this._router.navigate(['home'])
  }
}

export function delay(ms: number) {
  console.log("effettuo il delay")
  return new Promise( resolve => setTimeout(resolve, ms) );
}
