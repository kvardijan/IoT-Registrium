import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ActivatedRoute } from '@angular/router';
import { UserManagerService } from '../user-manager-service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-edit-device',
  standalone: true,
  imports: [FormsModule, MatSelectModule, MatFormFieldModule],
  templateUrl: './edit-device.html',
  styleUrl: './edit-device.scss'
})
export class EditDevice implements OnInit {
  id = '';
  serialNumber = '';
  message = '';
  model = '';
  manufacturer = '';
  firmware = '';
  type = -1;
  status = -1;
  location = -1;
  @Output() deviceUpdated = new EventEmitter<void>();

  types: any[] = [];
  statuses: any[] = [];
  locations: any[] = [];

  constructor(private route: ActivatedRoute, private http: HttpClient, public userManager: UserManagerService) { }

  ngOnInit(): void {
    this.serialNumber = this.route.snapshot.paramMap.get('serialNumber') || '';
    console.log(this.serialNumber);
    this.fetchDevice();
    this.fetchTypes();
    this.fetchStatuses();
    this.fetchLocations();
  }

  fetchDevice() {
    this.http.get<any>(environment.deviceApi + '/serial/' + this.serialNumber)
      .subscribe({
        next: (response) => {
          if (response.success) {
            console.log(response.data);
            this.id = response.data.id;
            this.model = response.data.model;
            this.manufacturer = response.data.manufacturer;
            this.firmware = response.data.firmwareVersion;
            this.type = response.data.typeId;
            this.status = response.data.statusId;
            this.location = response.data.location;
          } else {
            console.error('Failed to fetch device:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching device', err);
        }
      });
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


  editDevice() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };
    this.message = '';

    const body: any = {
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

    this.http.patch<{ success: boolean; data: string; error: string }>(environment.deviceApi + this.id, body, { headers })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.message = 'Device edited successfully.';
            this.deviceUpdated.emit();
          } else {
            this.message = 'Error with device info editing: ' + response.error;
          }
        },
        error: (err) => {
          this.message = 'Error with device info editing!';
          console.log(err);
        }
      });
  }
}
