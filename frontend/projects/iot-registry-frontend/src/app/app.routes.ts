import { Routes } from '@angular/router';
import { DevicesTable } from './devices-table/devices-table';
import { NotFound } from './not-found/not-found';
import { Login } from './login/login';

export const routes: Routes = [
    {path: 'devicesTable', component: DevicesTable},
    {path: 'login', component: Login},
    {path: '', component: DevicesTable},
    {path: '**', component: NotFound}
];
