import {Routes} from '@angular/router';
import { AuthGuard } from './_guards/auth.guard';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { MemberRegistrationComponent } from './members/member-registration/member-registration.component';

// Portfolio Components
import { PortfolioListResolver } from './_resolvers/portfolio-list.resolver';
import { PortfolioListComponent } from './portfolio/portfolio-list/portfolio-list.component';
import { PortfolioDetailComponent } from './portfolio/portfolio-detail/portfolio-detail.component';
import { PortfolioCreateComponent } from './portfolio/portfolio-create/portfolio-create.component';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'home', component: HomeComponent},
    {path: 'member/login', component: LoginComponent},
    {path: 'member/register', component: MemberRegistrationComponent},
    {path: 'portfolio', component: PortfolioListComponent,
        resolve: {portfolios: PortfolioListResolver}},
    {path: 'portfolio/detail', component: PortfolioDetailComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            {path: 'portfolio/create', component: PortfolioCreateComponent},
            {path: 'portfolio/edit', component: PortfolioCreateComponent},
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'},
];
