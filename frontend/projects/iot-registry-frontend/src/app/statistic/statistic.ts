import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserManagerService } from '../user-manager-service';
import { environment } from '../environments/environment';
import { MatAutocomplete } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Observable, firstValueFrom } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';

interface Device {
  id: number;
  serialNumber: string;
  model: string;
  manufacturer?: string;
  typeId: number;
  type: string;
  statusId: number;
  status: string;
  firmwareVersion: string;
  location: number;
  lastSeen: string;
}

@Component({
  selector: 'app-statistic',
  standalone: true,
  imports: [MatAutocomplete, MatFormFieldModule, MatAutocompleteModule, MatInputModule, ReactiveFormsModule, CommonModule],
  templateUrl: './statistic.html',
  styleUrl: './statistic.scss'
})
export class Statistic implements OnInit {
  active = 0;
  idle = 0;
  deactivated = 0;
  error = 0;
  total = 0;
  activePercent = 0;
  idlePercent = 0;
  deactivatedPercent = 0;
  errorPercent = 0;
  totalPercent = 0;

  devices: Device[] = [];
  deviceControl = new FormControl('');
  filteredDevices!: Observable<Device[]>;

  showTemperature = false;
  showHumidity = false;
  showSmartBin = false;
  temperatureData: { max: number; min: number; avg: number } | null = null;
  humidityData: { max: number; min: number; avg: number } | null = null;
  smartBinData: { percentageFull: number } | null = null;

  constructor(private http: HttpClient, public userManager: UserManagerService) { }

  ngOnInit(): void {
    this.fetchStatusStatistic();
    this.fetchDevices();
  }

  fetchStatusStatistic() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };

    this.http.get<any>(environment.statisticApi, { headers })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.active = response.data.active;
            this.idle = response.data.idle;
            this.deactivated = response.data.deactivated;
            this.error = response.data.error;
            this.total = response.data.active + response.data.idle + response.data.deactivated + response.data.error;

            this.activePercent = Math.round((this.active / this.total) * 10000) / 100;
            this.idlePercent = Math.round((this.idle / this.total) * 10000) / 100;
            this.deactivatedPercent = Math.round((this.deactivated / this.total) * 10000) / 100;
            this.errorPercent = Math.round((this.error / this.total) * 10000) / 100;
          } else {
            console.error('Failed to fetch status statistic:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching status statistic', err);
        }
      });
  }

  fetchDevices() {
    this.http.get<any>(environment.deviceApi)
      .subscribe({
        next: (response) => {
          if (response.success) {
            console.log(response.data);
            this.devices = response.data as Device[];

            this.filteredDevices = this.deviceControl.valueChanges.pipe(
              startWith(''),
              map((value: string | Device | null) => typeof value === 'string' ? value : value?.serialNumber ?? ''),
              map((serial: string) => serial ? this.filterDevices(serial) : this.devices.slice())
            );
          } else {
            console.error('Failed to fetch devices:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching devices', err);
        }
      });
  }

  private filterDevices(serial: string): Device[] {
    const filterValue = serial.toLowerCase();
    return this.devices.filter(d => d.serialNumber.toLowerCase().includes(filterValue));
  }

  displayDevice(device: Device): string {
    return device ? device.serialNumber : '';
  }

  onDeviceSelected(event: MatAutocompleteSelectedEvent) {
    const device = event.option.value;

    this.determineDeviceTypeAndFetch(device.serialNumber, device.typeId);
  }

  async fetchDeviceStatistic(serial: string, deviceType: string) {
    const jwt = this.userManager.getToken();
    const headers = { Authorization: 'Bearer ' + jwt };

    try {
      const response = await firstValueFrom(
        this.http.get<any>(`${environment.statisticApi}/${deviceType}/${serial}`, { headers })
      );

      if (response.success) {
        return response.data;
      } else {
        console.error('Failed to fetch device statistic:', response.error);
        return null;
      }
    } catch (err) {
      console.error('Error fetching device statistic', err);
      return null;
    }
  }

  async determineDeviceTypeAndFetch(serial: string, typeid: number) {
    this.showTemperature = false;
    this.showHumidity = false;
    this.showSmartBin = false;

    switch (typeid) {
      case 1: { // temperature
        const data = await this.fetchDeviceStatistic(serial, 'temperature');
        if (data != null) {
          this.processTemperatureData(data);
          this.showTemperature = true;
        } else {
          console.log("Failed fetching temperature statistic.");
        }
        break;
      }
      case 2: { // humidity
        const data = await this.fetchDeviceStatistic(serial, 'humidity');
        if (data != null) {
          this.processHumidityData(data);
          this.showHumidity = true;
        } else {
          console.log("Failed fetching humidity statistic.");
        }
        break;
      }
      case 4: { // smartbin
        const data = await this.fetchDeviceStatistic(serial, 'smartbin');
        if (data != null) {
          this.processSmartBinData(data);
          this.showSmartBin = true;
        } else {
          console.log("Failed fetching smartbin statistic.");
        }
        break;
      }
      default: { // not supported
        console.log("Device type not supported.");
      }
    }
  }

  processTemperatureData(data: any) {
    console.log(data);
    this.temperatureData = {
      max: data.max,
      min: data.min,
      avg: data.avg
    };
  }

  processHumidityData(data: any) {
    this.humidityData = {
      max: data.max,
      min: data.min,
      avg: data.avg
    };
  }

  processSmartBinData(data: any) {
    console.log(data);
    this.smartBinData = {
      percentageFull: data.percentageFull
    };
  }
}
