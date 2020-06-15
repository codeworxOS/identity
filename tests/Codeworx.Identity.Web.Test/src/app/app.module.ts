import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { OAuthService, AuthConfig, OAuthModule } from 'angular-oauth2-oidc';
import { HttpClientModule } from '@angular/common/http';
import { AuthGuard } from './auth-guard';
import { UrlHandlingStrategy } from '@angular/router';

const config: AuthConfig = {
  clientId: 'B45ABA81-AAC1-403F-93DD-1CE42F745ED2',
  issuer: './',
  postLogoutRedirectUri: 'abc',
  oidc: true,
  responseType: 'code',
  requireHttps: true
}

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    OAuthModule.forRoot(),
    HttpClientModule,
  ],
  providers: [
    AuthGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { 
  constructor(
      oauthService :OAuthService,
    ) {
    config.issuer = window.location.origin;
    config.redirectUri = window.location.origin;
    config.logoutUrl = window.location.origin + '/account/logout?returnUrl=' + encodeURIComponent(window.location.origin);
    oauthService.configure(config);
  }
}
