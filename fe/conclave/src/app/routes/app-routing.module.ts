import { NgModule } from '@angular/core';    
import { Routes, RouterModule } from '@angular/router';

import {AuthGuard} from '../core/authentication/auth.guard';
import { DashboardComponent } from '../views/dashboard/dashboard.component';    
import { LoginComponent } from '../views/login/login.component';
import {InvalidUrlComponent} from '../views/error/client/404/invalidurl.component';

import {TosComponent} from '../views/tos/tos.component';
import {PrivacyComponent} from '../views/privacy/privacy.component';
import {AboutComponent} from '../views/about/about.component';

export const routes: Routes = [    
  {    
    path: '',    
    redirectTo: 'login',    
    pathMatch: 'full',   
  },    
  {    
    path: 'login',    
    component: LoginComponent,    
    data: {    
      title: 'Login Page'    
    },
  },
  {    
    path: 'about',    
    component: AboutComponent,    
    data: {    
      title: 'About Us'    
    }    
  },  
  
  {    
    path: 'privacy',    
    component: PrivacyComponent,    
    data: {    
      title: 'Privacy Policy'    
    }    
  },  

  {    
    path: 'terms',    
    component: TosComponent,    
    data: {    
      title: 'Terms and Conditions'    
    }    
  },  
  {    
    path: 'dashboard',    
    component: DashboardComponent,    
    data: {    
      title: 'Dashboard Page'    
    },
    canActivate: [AuthGuard]
  },
  {
    path:'**',
    component:InvalidUrlComponent
  }  
];    
@NgModule({    
  imports: [RouterModule.forRoot(routes,{ enableTracing: true })],    
  exports: [RouterModule]    
})    
export class AppRoutingModule { }