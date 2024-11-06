import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { OAuthService, AuthConfig, OAuthModule } from 'angular-oauth2-oidc';
import { HttpClientModule, HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { AuthGuard } from './auth-guard';
import { UrlHandlingStrategy } from '@angular/router';
import { AuthInterceptor } from './auth-interceptor';
import { TenantSelectorComponent } from './tenant-selector.component';
import { FormsModule } from '@angular/forms';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AppRootComponent } from './app-root.component';
import { OrderListComponent } from './order/order-list.component';
import { OrderDetailComponent } from './order/order-detail.component';
import { RefreshInterceptor } from './refresh.interceptor';

const config: AuthConfig = {
  clientId: 'b45aba81aac1403f93dd1ce42f745ed2',
  issuer: './',
  postLogoutRedirectUri: 'abc',
  oidc: true,
  responseType: 'code',
  requireHttps: true,
  scope: 'openid profile tenant offline_access group_names external_token:access_token'
}

@NgModule({
  declarations: [
    AppComponent,
    TenantSelectorComponent,
    DashboardComponent,
    AppRootComponent,
    OrderListComponent,
    OrderDetailComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    OAuthModule.forRoot(),
    FormsModule,
  ],
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    AuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: RefreshInterceptor,
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
