import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html'
})
export class DashboardComponent
    implements OnInit {
    constructor(private http: HttpClient) {

    }

    public name: string;

    async ngOnInit(): Promise<void> {
        var response = await this.http.get("/api/me", { responseType: 'text' }).toPromise();
        this.name = response;
    }


}