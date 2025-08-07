import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-device-events',
  standalone: true,
  imports: [],
  templateUrl: './device-events.html',
  styleUrl: './device-events.scss'
})
export class DeviceEvents implements OnInit {
  serialNumber: string = '';

  constructor(private route: ActivatedRoute){}

  ngOnInit(): void {
    this.serialNumber = this.route.snapshot.paramMap.get('serialNumber') || '';
    console.log('Serial number from route:', this.serialNumber);
  }
}
