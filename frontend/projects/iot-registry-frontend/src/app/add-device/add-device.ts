import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { HttpClient } from '@angular/common/http';
import { UserManagerService } from '../user-manager-service';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-add-device',
  imports: [CommonModule, FormsModule, MatSelectModule, MatFormFieldModule],
  templateUrl: './add-device.html',
  styleUrl: './add-device.scss'
})
export class AddDevice implements OnInit {
  message = '';
  serialNumber = '';
  model = '';
  manufacturer = '';
  firmware = '';
  type = -1;
  status = -1;
  location = -1;

  types: any[] = [];
  statuses: any[] = [];
  locations: any[] = [];

  constructor(private http: HttpClient, public userManager: UserManagerService) { }

  ngOnInit(): void {
    this.fetchStatuses();
    this.fetchTypes();
    this.fetchLocations();
  }

  fetchStatuses() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };

    this.http.get<any>(environment.deviceApi + '/statuses', { headers })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.statuses = response.data;
            console.log(this.statuses);
          } else {
            console.error('Failed to fetch statuses:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching statuses', err);
        }
      });
  }

  fetchTypes() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };

    this.http.get<any>(environment.deviceApi + '/types', { headers })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.types = response.data;
            console.log(this.types);
          } else {
            console.error('Failed to fetch types:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching types', err);
        }
      });
  }

  fetchLocations() {
    this.http.get<any>(environment.locationApi)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.locations = response.data;
          } else {
            console.error('Failed to fetch locations:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching locations', err);
        }
      });
  }

  registerDevice() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };
    this.message = '';

    const body: any = {
      serialNumber: this.serialNumber,
      model: this.model,
      type: this.type,
      status: this.status,
      firmwareVersion: this.firmware
    };

    if (this.manufacturer) {
      body.manufacturer = this.manufacturer;
    }

    if (this.location) {
      body.location = this.location;
    }

    this.http.post<{ success: boolean; data: string; error: string }>(environment.deviceApi, body, { headers })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.message = 'Device registered successfully.';
          } else {
            this.message = 'Error with device registration: ' + response.error;
          }
        },
        error: (err) => {
          this.message = 'Error with device registration!';
          console.log(err);
        }
      });
  }
}
