import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { OAuthService } from "angular-oauth2-oidc";

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html'
})
export class DashboardComponent
    implements OnInit {
    constructor(private http: HttpClient, private oauthService: OAuthService) {

    }

    public name: string;

    async ngOnInit(): Promise<void> {
        var response = await this.http.get("/api/me", { responseType: 'text' }).toPromise();
        this.name = response;
    }

    public async refreshToken(): Promise<void> {
        let response = await this.oauthService.refreshToken();
        alert(JSON.stringify(response));
    }

}