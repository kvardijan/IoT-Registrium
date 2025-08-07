import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { HttpClient } from '@angular/common/http';
import { UserManagerService } from '../user-manager-service';

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
}
