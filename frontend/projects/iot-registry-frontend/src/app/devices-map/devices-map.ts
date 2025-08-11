import { Component, AfterViewInit } from '@angular/core';
import Map from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import { fromLonLat } from 'ol/proj';

@Component({
  selector: 'app-devices-map',
  standalone: true,
  imports: [],
  templateUrl: './devices-map.html',
  styleUrl: './devices-map.scss'
})
export class DevicesMap {
  varazdinLatitude = 46.3057;
  varazdinLongitude = 16.3366;

  ngAfterViewInit(): void {
    new Map({
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
}
