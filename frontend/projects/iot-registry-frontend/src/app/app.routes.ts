import { Routes } from '@angular/router';
import { DevicesTable } from './devices-table/devices-table';
import { NotFound } from './not-found/not-found';
import { Login } from './login/login';
import { LocationsTable } from './locations-table/locations-table';
import { authGuard } from './auth.guard';

export const routes: Routes = [
    {path: 'devicestable', component: DevicesTable},
    {path: 'login', component: Login},
    { path: 'locations', component: LocationsTable, canActivate: [authGuard] },
    {path: '', component: DevicesTable},
    {path: '**', component: NotFound}
];
