import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { OAuthService, AuthConfig, OAuthModule } from 'angular-oauth2-oidc';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthGuard } from './auth-guard';
import { UrlHandlingStrategy } from '@angular/router';
import { AuthInterceptor } from './auth-interceptor';
import { TenantSelectorComponent } from './tenant-selector.component';
import { FormsModule } from '@angular/forms';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AppRootComponent } from './app-root.component';

const config: AuthConfig = {
  clientId: 'B45ABA81-AAC1-403F-93DD-1CE42F745ED2',
  issuer: './',
  postLogoutRedirectUri: 'abc',
  oidc: true,
  responseType: 'code',
  requireHttps: true,
  scope: 'openid profile tenant'
}

@NgModule({
  declarations: [
    AppComponent,
    TenantSelectorComponent,
    DashboardComponent,
    AppRootComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    OAuthModule.forRoot(),
    HttpClientModule,
    FormsModule,
  ],
  providers: [
    AuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppRootComponent]
})
export class AppModule {
  constructor(
    oauthService: OAuthService,
  ) {
    config.issuer = window.location.origin;
    config.redirectUri = window.location.origin;
    config.logoutUrl = window.location.origin + '/account/logout?returnUrl=' + encodeURIComponent(window.location.origin);
    oauthService.configure(config);
  }
}
