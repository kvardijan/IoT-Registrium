import { Routes } from '@angular/router';
import { DevicesTable } from './devices-table/devices-table';
import { NotFound } from './not-found/not-found';
import { Login } from './login/login';
import { LocationsTable } from './locations-table/locations-table';
import { AddDevice } from './add-device/add-device';
import { authGuard } from './auth.guard';
import { DeviceEvents } from './device-events/device-events';
import { DevicesMap } from './devices-map/devices-map';

export const routes: Routes = [
    { path: 'devicestable', component: DevicesTable },
    { path: 'devicesmap', component: DevicesMap },
    { path: 'login', component: Login },
    { path: 'locations', component: LocationsTable, canActivate: [authGuard] },
    { path: 'adddevice', component: AddDevice, canActivate: [authGuard] },
    { path: 'devices/:serialNumber/events', component: DeviceEvents, canActivate: [authGuard] },
    { path: '', component: DevicesTable },
    { path: '**', component: NotFound }
];
