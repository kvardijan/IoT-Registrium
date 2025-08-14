import { Component, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserManagerService } from '../user-manager-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import OLMap from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import { fromLonLat, toLonLat } from 'ol/proj';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import { Style, Fill, Stroke, Circle as CircleStyle } from 'ol/style';
import { ActivatedRoute } from '@angular/router';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-edit-location',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-location.html',
  styleUrl: './edit-location.scss'
})
export class EditLocation implements OnInit, AfterViewInit {
  locationId = '';
  message = '';
  address = '';
  description = '';
  longitude = 15.8724;
  latitude = 46.1605;
  private map!: OLMap;
  private markerLayer!: VectorLayer<VectorSource>;

  constructor(private http: HttpClient, public userManager: UserManagerService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.locationId = this.route.snapshot.paramMap.get('locationId') || '';
    console.log(this.locationId);
    this.fetchLocation();
  }

  ngAfterViewInit() {
    this.map = new OLMap({
      target: 'map',
      layers: [
        new TileLayer({
          source: new OSM()
        })
      ],
      view: new View({
        center: fromLonLat([this.longitude, this.latitude]),
        zoom: 14
      }),
      controls: [],
    });

    this.markerLayer = new VectorLayer({
      source: new VectorSource(),
      style: new Style({
        image: new CircleStyle({
          radius: 7,
          fill: new Fill({ color: '#1976d2' }),
          stroke: new Stroke({ color: '#000000ff', width: 2 })
        })
      })
    });
    this.map.addLayer(this.markerLayer);

    this.map.on('singleclick', (event) => {
      const coords = toLonLat(event.coordinate);
      this.longitude = coords[0];
      this.latitude = coords[1];

      console.log('Coordinates:', this.longitude, this.latitude);

      this.placeMarker(this.longitude, this.latitude);

      this.http.get<any>(
        `https://nominatim.openstreetmap.org/reverse?format=json&lat=${this.latitude}&lon=${this.longitude}&addressdetails=1`
      ).subscribe({
        next: (data) => {
          this.address = '';
          this.description = '';
          console.log('data:', data);
          if (data && !data.error) {
            if (data.name) {
              this.description = data.name;
            }
            if (data.address.road) {
              this.address += data.address.road + ', ';
            }
            if (data.address.postcode) {
              this.address += data.address.postcode + ', ';
            }
            if (data.address.town) {
              this.address += data.address.town + ', ';
            }
            if (data.address.village) {
              this.address += data.address.village + ', ';
            }
            if (data.address.city) {
              this.address += data.address.city + ', ';
            }
            this.address = this.address.substring(0, this.address.length - 2);
          } else {
            this.address = 'Address not found';
          }
        },
        error: (err) => {
          console.error('Error fetching address', err);
          this.address = 'Error fetching address';
        }
      });
    });
  }

  private placeMarker(lon: number, lat: number) {
    const feature = new Feature({
      geometry: new Point(fromLonLat([lon, lat]))
    });

    this.markerLayer.getSource()?.clear();
    this.markerLayer.getSource()?.addFeature(feature);
  }

  fetchLocation() {
    this.http.get<any>(environment.locationApi + '/api/location/' + this.locationId)
      .subscribe({
        next: (response) => {
          if (response.success) {
            console.log(response.data);
            this.address = response.data.address;
            this.longitude = response.data.longitude;
            this.latitude = response.data.latitude;
            this.description = response.data.description;

            this.placeMarker(this.longitude, this.latitude);
            this.map.getView().setCenter(fromLonLat([this.longitude, this.latitude]));
          } else {
            console.error('Failed to fetch location:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching location', err);
        }
      });
  }

  editLocation() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };
    this.message = '';

    const body: any = {
      latitude: this.latitude.toString(),
      longitude: this.longitude.toString(),
      address: this.address,
      description: this.description
    };

    this.http.patch<{ success: boolean; data: string; error: string }>(environment.locationApi + '/api/location/' + this.locationId, body, { headers })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.message = 'Location edited successfully.';
          } else {
            this.message = 'Error with location editing: ' + response.error;
          }
        },
        error: (err) => {
          this.message = 'Error with location editing!';
          console.log(err);
        }
      });
  }
}
