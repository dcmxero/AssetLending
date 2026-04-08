import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard';
import { AssetCreateComponent } from './components/assets/asset-create';
import { AssetDetailComponent } from './components/assets/asset-detail';
import { LoanListComponent } from './components/loans/loan-list';
import { UserListComponent } from './components/users/user-list';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'assets', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'assets/new', component: AssetCreateComponent },
  { path: 'assets/:id', component: AssetDetailComponent },
  { path: 'loans', component: LoanListComponent },
  { path: 'users', component: UserListComponent },
];
