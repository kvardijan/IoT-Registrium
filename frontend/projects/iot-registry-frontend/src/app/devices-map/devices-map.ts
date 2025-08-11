import { Component, AfterViewInit, OnInit } from '@angular/core';
import OLMap from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import { fromLonLat } from 'ol/proj';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-devices-map',
  standalone: true,
  imports: [],
  templateUrl: './devices-map.html',
  styleUrl: './devices-map.scss'
})
export class DevicesMap implements AfterViewInit, OnInit {
  varazdinLatitude = 46.3057;
  varazdinLongitude = 16.3366;
  devices: any[] = [];
  locationsMap: Map<number, string> = new Map<number, string>();
  locations: any[] = [];

  constructor(private router: Router, private http: HttpClient) { }

  ngOnInit(): void {
    this.loadDevicesAndLocations();
  }

  ngAfterViewInit(): void {
    new OLMap({
      target: 'map',
      layers: [
        new TileLayer({
          source: new OSM()
        })
      ],
      view: new View({
        center: fromLonLat([this.varazdinLongitude, this.varazdinLatitude]),
        zoom: 14
      }),
      controls: [],
    });
  }

  loadDevicesAndLocations() {
    this.fetchLocations(() => {
      this.fetchDevices();
    })
  }

  fetchLocations(callback: () => void) {
    this.http.get<any>('http://localhost:5261/api/location')
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.locations = response.data;
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
}
