// angular modules and components
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { appRoutes } from './routes';
import { ErrorInterceptorProvider } from './_services/error.interceptor';

import { BsDatepickerModule, PaginationModule } from 'ngx-bootstrap';

// page components
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';

import { LoginComponent } from './login/login.component';
import { MemberRegistrationComponent } from './members/member-registration/member-registration.component';

import { PortfolioListResolver } from './_resolvers/portfolio-list.resolver';
import { PortfolioListComponent } from './portfolio/portfolio-list/portfolio-list.component';
import { PortfolioCreateComponent } from './portfolio/portfolio-create/portfolio-create.component';

// service imports
import { AlertifyService } from './_services/alertify.service';
import { UtilityService } from './_services/utility.service';


@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      LoginComponent,
      MemberRegistrationComponent,
      PortfolioListComponent,
      PortfolioCreateComponent

   ],
   imports: [
      BrowserModule,
      ReactiveFormsModule,
      HttpClientModule,
      PaginationModule.forRoot(),
      RouterModule.forRoot(appRoutes)
   ],
    providers: [
      AlertifyService,
      UtilityService,
      ErrorInterceptorProvider,
      PortfolioListResolver
    ],
    bootstrap: [
        AppComponent
   ]
})
export class AppModule { }
