import { Component, OnInit } from "@angular/core";
import { Order } from './order';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'order-list',
    templateUrl: './order-list.component.html'
})
export class OrderListComponent implements OnInit
{
    public async ngOnInit(): Promise<void>
    {
        var response = await this.http.get<Order[]>("/api/Demo").toPromise();
        this.orders = response;
    }
    
    public orders: Order[] = [];

    public selectedOrder: Order;

    constructor(private http: HttpClient) 
    {

    }

    public async addNew()
    {
        let order: Order = { orderDescription : 'new', orderDate: new Date() };
        this.orders.push(order);
        this.selectedOrder = order;
    }

    public  select(item: Order)
    {
        this.selectedOrder = item;
    }

    public async delete(){
        await this.http.delete("/api/Demo/" + this.selectedOrder.id).toPromise();

        await this.ngOnInit();
    }
}