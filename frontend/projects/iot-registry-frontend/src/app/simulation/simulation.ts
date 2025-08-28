import { Component, OnInit } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { UserManagerService } from '../user-manager-service';

@Component({
  selector: 'app-simulation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './simulation.html',
  styleUrl: './simulation.scss'
})
export class Simulation implements OnInit {
  devices: any[] = [];

  constructor(private http: HttpClient, public userManager: UserManagerService) { }

  ngOnInit(): void {
    this.fetchDevices();
  }

  fetchDevices() {
    this.http.get<any>(environment.deviceApi)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.devices = response.data;
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

  onStopAllSimulationsClick(){}
}
