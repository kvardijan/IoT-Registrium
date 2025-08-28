import { Component, OnInit, OnDestroy } from '@angular/core';
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
export class Simulation implements OnInit, OnDestroy {
  devices: any[] = [];

  constructor(private http: HttpClient, public userManager: UserManagerService) { }

  ngOnInit(): void {
    this.fetchDevices();

    window.addEventListener('beforeunload', () => {
      this.stopAllSimulationsSilent();
    });
  }

  ngOnDestroy(): void {
    this.stopAllSimulationsSilent();
  }

  fetchDevices() {
    this.http.get<any>(environment.deviceApi)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.devices = response.data.map((d: any) => ({
              ...d,
              simulating: false
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
      case 'active': return 'green';
      case 'idle': return 'yellow';
      case 'deactivated': return 'gray';
      case 'error': return 'red';
      default: return '';
    }
  }

  toggleSimulation(device: any) {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };

    if (device.simulating) {
      this.http.post(environment.simulationApi + `/stop/${device.serialNumber}`, {}, { headers })
        .subscribe({
          next: () => device.simulating = false,
          error: (err) => console.error('Error stopping simulation', err)
        });
    } else {
      const body = { TypeId: device.typeId };
      this.http.post(environment.simulationApi + `/start/${device.serialNumber}`, body, { headers })
        .subscribe({
          next: () => device.simulating = true,
          error: (err) => console.error('Error starting simulation', err)
        });
    }
  }

  onStopAllSimulationsClick() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };

    this.http.post(environment.simulationApi + '/stop-all', {}, { headers })
      .subscribe({
        next: () => this.devices.forEach(d => d.simulating = false),
        error: (err) => console.error('Error stopping all simulations', err)
      });
  }

  private stopAllSimulationsSilent() {
    navigator.sendBeacon(environment.simulationApi + '/stop-all');
  }
}
