import { Routes } from '@angular/router';
import { DevicesTable } from './devices-table/devices-table';
import { NotFound } from './not-found/not-found';

export const routes: Routes = [
    {path: 'devicesTable', component: DevicesTable},
    {path: '', component: DevicesTable},
    {path: '**', component: NotFound}
];
