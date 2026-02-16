import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { Login } from './features/auth/login';
import { Home } from './features/home/home';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: '', canActivate: [authGuard], component: Home },
  { path: '**', redirectTo: '' },
];
