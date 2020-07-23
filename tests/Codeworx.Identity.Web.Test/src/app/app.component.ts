import { Component } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  template: `
    <!--The content below is only a placeholder and can be replaced.-->
    <div style="text-align:center" class="content">
      <h1>
        Welcome to {{title}}!
      </h1>
      <button (click)="logout()">logout</button>
    </div>
    <router-outlet></router-outlet>
  `,
  styles: []
})
export class AppComponent {
  title = 'identity-test';

  /**
   *
   */
  constructor(private oauthService: OAuthService, private router: Router) {
  }

    public async logout(): Promise<void>
    {
        await this.oauthService.loadDiscoveryDocument();
        this.oauthService.logOut(true);

        let urlPromise: Promise<string> = (<any>this.oauthService).createLoginUrl({}, '', null, false, { "prompt": "select_account"});
        let url = await urlPromise;

        let identityUrl: string = this.oauthService.issuer;
        identityUrl = identityUrl.toLowerCase();

        this.oauthService.logoutUrl = identityUrl + '/account/logout?returnUrl=' + encodeURIComponent(url);
        this.oauthService.postLogoutRedirectUri = this.oauthService.logoutUrl;

        window.location.href = this.oauthService.logoutUrl;
  }
}
