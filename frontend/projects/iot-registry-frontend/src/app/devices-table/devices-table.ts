import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-devices-table',
    standalone: true, 
  imports: [CommonModule],
  templateUrl: './devices-table.html',
  styleUrl: './devices-table.scss'
})
export class DevicesTable implements OnInit {
  devices: any[] = [];
  locationsMap: Map<number, string> = new Map<number, string>();

  constructor(private http: HttpClient) {

  }

  ngOnInit(): void {
    this.loadDevicesAndLocations();
    
  }

  loadDevicesAndLocations(){
    this.fetchLocations(() => {
      this.fetchDevices();
    })
  }

  fetchLocations(callback: () => void) {
    this.http.get<any>('http://localhost:5261/api/location')
      .subscribe({
        next: (response) => {
          if (response.success) {
            response.data.forEach((loc: any) => {
              this.locationsMap.set(loc.id, loc.address);
            });
          } else {
            console.error('Failed to fetch locations:', response.error);
          }
          callback();
        },
        error: (err) => {
          console.error('Error fetching locations', err);
          callback();
        }
      });
  }

  fetchDevices() {
    this.http.get<any>('http://localhost:5208/api/device')
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.devices = response.data.map((device: any) => ({
              ...device,
              locationName: this.locationsMap.get(device.location) || 'Unknown'
            }));
          } else {
            console.error('Failed to fetch devices:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching devices', err);
        }
      });
  }

  getStatusClass(status: string): string {
  switch (status.toLowerCase()) {
    case 'active':
      return 'green';
    case 'idle':
      return 'yellow';
    case 'deactivated':
      return 'gray';
    case 'error':
      return 'red';
    default:
      return '';
  }
}
}
