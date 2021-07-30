import { Component, Input } from "@angular/core";
import { Order } from './order';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'order-detail',
    templateUrl: './order-detail.component.html'
})
export class OrderDetailComponent
{
    @Input("Order")
    public order: Order;

constructor(private http: HttpClient) {
    
}

    public async save() : Promise<void>
    {
        let savedOrder : Order;
        if(this.order.id){
            savedOrder = await this.http.post<Order>('api/Demo',this.order).toPromise();
        } else {
            savedOrder = await this.http.put<Order>('api/Demo',this.order).toPromise();
        }

        this.order.id = savedOrder.id;
        this.order.orderDate = savedOrder.orderDate;
        this.order.orderDescription = savedOrder.orderDescription;
        this.order.orderNumber = savedOrder.orderNumber;
    }
}