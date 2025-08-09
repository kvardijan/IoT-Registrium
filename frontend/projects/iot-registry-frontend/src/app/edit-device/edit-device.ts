import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ActivatedRoute } from '@angular/router';
import { UserManagerService } from '../user-manager-service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-edit-device',
  standalone: true,
  imports: [FormsModule, MatSelectModule, MatFormFieldModule],
  templateUrl: './edit-device.html',
  styleUrl: './edit-device.scss'
})
export class EditDevice implements OnInit {
  serialNumber = '';
  message = '';
  model = '';
  manufacturer = '';
  firmware = '';
  type = -1;
  status = -1;
  location = -1;

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
    this.http.get<any>('http://localhost:5208/api/device/serial/' + this.serialNumber)
      .subscribe({
        next: (response) => {
          if (response.success) {
            console.log(response.data);
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

    this.http.get<any>('http://localhost:5208/api/device/statuses', { headers })
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

    this.http.get<any>('http://localhost:5208/api/device/types', { headers })
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
    this.http.get<any>('http://localhost:5261/api/location')
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

  }
}
