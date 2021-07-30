import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OAuthService } from 'angular-oauth2-oidc';
import { Injectable } from '@angular/core';

@Injectable()
export class AuthInterceptor
    implements HttpInterceptor {

    constructor(private oauthService: OAuthService) {

    }

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (this.oauthService.hasValidAccessToken()) {
            return next.handle(req.clone({ headers: req.headers.set('Authorization', 'Bearer ' + this.oauthService.getAccessToken()) }));
        }

        return next.handle(req);
    }


}