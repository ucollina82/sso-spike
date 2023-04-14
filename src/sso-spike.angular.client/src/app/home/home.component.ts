import {Component, OnInit} from '@angular/core';
import {AuthService} from 'src/oidc/services';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  userInfo: string = '';

  constructor(readonly authService: AuthService) {

  }

  ngOnInit(): void {
    this.authService.getUserInfo().then(user => {this.userInfo = JSON.stringify(user)});
  }
}
