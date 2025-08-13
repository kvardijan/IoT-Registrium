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

@Component({
  selector: 'app-edit-location',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-location.html',
  styleUrl: './edit-location.scss'
})
export class EditLocation {
  message = '';
  address = '';
  description = '';
  longitude = 15.8724;
  latitude = 46.1605;

  constructor(private http: HttpClient, public userManager: UserManagerService) { }

  editLocation(){

  }
}
