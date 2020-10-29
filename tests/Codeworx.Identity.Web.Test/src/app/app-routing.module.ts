import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthGuard } from './auth-guard';
import { AppComponent } from './app.component';


const routes: Routes = [
  {
    path: '',
    component: AppComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: DashboardComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
