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
    ngOnInit(): void {
        throw new Error("Method not implemented.");
    }


}