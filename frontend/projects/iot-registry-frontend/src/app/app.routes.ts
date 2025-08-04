import { Routes } from '@angular/router';
import { DevicesTable } from './devices-table/devices-table';

export const routes: Routes = [
    {path: '', component: DevicesTable},
    {path: 'devicesTable', component: DevicesTable}
];
