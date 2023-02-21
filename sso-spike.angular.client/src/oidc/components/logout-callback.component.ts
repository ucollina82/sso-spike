import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthUrlConstantServiceToken, IAuthUrlConstantService } from '../services';
import { AuthService } from '../services/auth.service';

@Component({
    selector: 'signout-callback',
    template: ''
})
export class LogoutCallbackComponent implements OnInit {
    constructor(
        private readonly _auth2Service: AuthService,
        private readonly _router: Router
    ) {}



  async ngOnInit() {
    await this._auth2Service.finishLogout();
    this._router.navigate(['/'], { replaceUrl: true });
  }
}
