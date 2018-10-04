// angular modules and components
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { appRoutes } from './routes';
import { ErrorInterceptorProvider } from './_services/error.interceptor';

import { AuthGuard } from './_guards/auth.guard';
import { JwtModule } from '@auth0/angular-jwt';
import { BsDatepickerModule, PaginationModule } from 'ngx-bootstrap';

// service imports
import { AlertifyService } from './_services/alertify.service';
import { UtilityService } from './_services/utility.service';

/////////////////////
// Page components //
/////////////////////
import { NavComponent } from './nav/nav.component';

// contact me
import { ContactMeComponent } from './contact-me/contact-me.component';

// log
import { LogListResolver } from './_resolvers/log-list.resolver';
import { LogComponent } from './log/log.component';

// home
import { HomeComponent } from './home/home.component';

// Members
import { LoginComponent } from './login/login.component';
import { MemberRegistrationComponent } from './members/member-registration/member-registration.component';

// Portfolio
import { PortfolioListResolver } from './_resolvers/portfolio-list.resolver';
import { PortfolioListComponent } from './portfolio/portfolio-list/portfolio-list.component';
import { PortfolioDetailComponent } from './portfolio/portfolio-detail/portfolio-detail.component';
import { PortfolioCreateComponent } from './portfolio/portfolio-create/portfolio-create.component';


export function tokenGetter() {
  return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      LogComponent,
      HomeComponent,
      LoginComponent,
      MemberRegistrationComponent,
      PortfolioListComponent,
      PortfolioDetailComponent,
      PortfolioCreateComponent,
      ContactMeComponent
   ],
   imports: [
      BrowserModule,
      ReactiveFormsModule,
      HttpClientModule,
      PaginationModule.forRoot(),
      RouterModule.forRoot(appRoutes, {useHash: true}),
      JwtModule.forRoot({
        config: {
          tokenGetter: tokenGetter,
          whitelistedDomains: ['localhost:5000'],
          blacklistedRoutes: ['localhost:5000/api/auth']
        }
      })
   ],
    providers: [
      AuthGuard,
      ErrorInterceptorProvider,
      AlertifyService,
      PortfolioListResolver,
      LogListResolver,
      UtilityService

    ],
    bootstrap: [
        AppComponent
   ]
})
export class AppModule { }
