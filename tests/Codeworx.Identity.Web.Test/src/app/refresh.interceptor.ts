import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OAuthService } from 'angular-oauth2-oidc';
import { Injectable } from '@angular/core';

export class RefreshInterceptor implements HttpInterceptor {
    
        public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
            if(req.body && req.method == "POST" && req.body instanceof HttpParams){
                if (req.body.has('grant_type') && req.body.get('grant_type') == 'refresh_token' && req.body.has('scope')) {
                    req = req.clone({ 'body': req.body.delete('scope')});
                }
            }
    
            return next.handle(req);
        }
}