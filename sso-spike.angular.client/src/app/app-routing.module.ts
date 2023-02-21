import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginCallbackComponent, LogoutCallbackComponent, SilentCallbackComponent } from 'src/oidc/components';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: 'signin-callback',
    component: LoginCallbackComponent
  },
  {
    path: 'signout-callback',
    component: LogoutCallbackComponent,
  },
  {
    path: 'silent-callback',
    component: SilentCallbackComponent,
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
