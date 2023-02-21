import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthGuard } from './guards';
import { AuthInterceptor } from './interceptors';
import { LoginCallbackComponent, LogoutCallbackComponent, SilentCallbackComponent } from './components';
import { AuthService } from './services/auth.service';

@NgModule({
    declarations: [
        LoginCallbackComponent,
        LogoutCallbackComponent,
        SilentCallbackComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
    ],
    providers: [
        AuthGuard,
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
        AuthService,
    ],
    exports: [
        LoginCallbackComponent,
        LogoutCallbackComponent,
        SilentCallbackComponent,
    ]
})
export class NgAuthModule {}
