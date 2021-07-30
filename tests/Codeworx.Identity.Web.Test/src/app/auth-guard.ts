import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc'

@Injectable()
export class AuthGuard
    implements CanActivate {
    constructor(private oauthService: OAuthService) {

    }

    public async canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Promise<boolean> {
        await this.oauthService.loadDiscoveryDocument();

        if (this.oauthService.hasValidAccessToken()) {
            return true;
        }

        var loggedin = await this.oauthService.tryLogin()

        if (loggedin && this.oauthService.hasValidAccessToken()) {
            return true;
        }

        this.oauthService.initLoginFlow();
        return false;
    }

}