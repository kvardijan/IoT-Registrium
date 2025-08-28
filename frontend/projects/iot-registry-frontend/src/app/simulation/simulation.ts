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
  message = '';

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
    if (device.typeId == 1 || device.typeId == 2 || device.typeId == 4) {
      const jwt = this.userManager.getToken();
      const headers = {
        Authorization: 'Bearer ' + jwt
      };

      if (device.simulating) {
        this.http.post(environment.simulationApi + `/stop/${device.serialNumber}`, {}, { headers })
          .subscribe({
            next: () => {
              device.simulating = false;
              this.message = 'Stopped simulation for device ' + device.serialNumber;
            },
            error: (err) => console.error('Error stopping simulation', err)
          });
      } else {
        const body = { TypeId: device.typeId };
        this.http.post(environment.simulationApi + `/start/${device.serialNumber}`, body, { headers })
          .subscribe({
            next: () => {
              device.simulating = true;
              this.message = 'Started simulation for device ' + device.serialNumber;
            },
            error: (err) => console.error('Error starting simulation', err)
          });
      }
    }
  }

  onStopAllSimulationsClick() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };

    this.http.post(environment.simulationApi + '/stop-all', {}, { headers })
      .subscribe({
        next: () => {
          this.devices.forEach(d => d.simulating = false);
          this.message = 'Stopped all simulations';
        },
        error: (err) => console.error('Error stopping all simulations', err)
      });
  }

  private stopAllSimulationsSilent() {
    const jwt = this.userManager.getToken();
    fetch(environment.simulationApi + '/stop-all', {
      method: 'POST',
      headers: {
        'Authorization': 'Bearer ' + jwt,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({}),
      keepalive: true
    }).catch(err => console.error('Failed stopping simulations on unload', err));
  }
}
