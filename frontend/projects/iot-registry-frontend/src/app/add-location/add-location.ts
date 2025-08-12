import { Component, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserManagerService } from '../user-manager-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import OLMap from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import { fromLonLat } from 'ol/proj';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import { Style, Fill, Stroke, Circle as CircleStyle } from 'ol/style';

@Component({
  selector: 'app-add-location',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-location.html',
  styleUrl: './add-location.scss'
})
export class AddLocation implements OnInit, AfterViewInit {
  message = '';
  address = '';
  description = '';
  longitude = 15.8724;
  latitude = 46.1605;
  private map!: OLMap;
  private markerLayer?: VectorLayer;

  constructor(private http: HttpClient, public userManager: UserManagerService) { }

  ngOnInit(): void {

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
        center: fromLonLat([this.longitude, this.latitude]),
        zoom: 14
      }),
      controls: [],
    });
  }

  addLocation() {

  }
}
