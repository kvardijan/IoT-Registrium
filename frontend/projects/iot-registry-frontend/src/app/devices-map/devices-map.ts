import { Component, AfterViewInit, OnInit } from '@angular/core';
import OLMap from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import { fromLonLat } from 'ol/proj';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
//import { Icon, Style } from 'ol/style';
import { Style, Fill, Stroke, Circle as CircleStyle } from 'ol/style';

@Component({
  selector: 'app-devices-map',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './devices-map.html',
  styleUrl: './devices-map.scss'
})
export class DevicesMap implements AfterViewInit, OnInit {
  krapinaLatitude = 46.1605;
  krapinaLongitude = 15.8724;
  devices: any[] = [];
  locationsMap: Map<number, string> = new Map<number, string>();
  locations: any[] = [];
  private map!: OLMap;
  private markerLayer?: VectorLayer;

  constructor(private router: Router, private http: HttpClient) { }

  ngOnInit(): void {
    this.loadDevicesAndLocations();
  }

  ngAfterViewInit(): void {
    this.map = new OLMap({
      target: 'map',
      layers: [
        new TileLayer({
          source: new OSM()
        })
      ],
      view: new View({
        center: fromLonLat([this.krapinaLongitude, this.krapinaLatitude]),
        zoom: 14
      }),
      controls: [],
    });

    this.map.on('click', (evt) => {
      const feature = this.map.forEachFeatureAtPixel(evt.pixel, (feat) => feat);
      if (feature) {
        const locationId = feature.get('locationId');
        const locationName = feature.get('locationName');
        if (locationId !== undefined && locationName !== undefined) {
          this.onMarkerClick(locationId, locationName);
        }
      }
    });


    if (this.locations.length) {
      this.addLocationMarkers();
    }
  }

  loadDevicesAndLocations() {
    this.fetchLocations(() => {
      this.fetchDevices();
      if (this.map) {
        this.addLocationMarkers();
      } else {
        console.log('no map yet');
      }
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

  private addLocationMarkers() {
    if (!this.map || !this.locations?.length) return;

    if (this.markerLayer) {
      this.map.removeLayer(this.markerLayer);
    }

    const features = this.locations.map(loc => {
      const coords = fromLonLat([loc.longitude, loc.latitude]);
      const f = new Feature({
        geometry: new Point(coords),
        locationId: loc.id,
        locationName: loc.address
      });
      return f;
    });

    const vectorSource = new VectorSource({ features });

    const markerStyle = new Style({
      image: new CircleStyle({
        radius: 8,
        fill: new Fill({ color: '#1976d2' }),
        stroke: new Stroke({ color: '#000000ff', width: 2 })
      })
    });

    this.markerLayer = new VectorLayer({
      source: vectorSource,
      style: markerStyle
    });

    this.map.addLayer(this.markerLayer);
  }

  onMarkerClick(locationId: number, locationName: string) {
    console.log('Marker clicked:', locationId, locationName);
  }
}
