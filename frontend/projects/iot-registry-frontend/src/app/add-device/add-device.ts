import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-device',
  imports: [CommonModule, FormsModule],
  templateUrl: './add-device.html',
  styleUrl: './add-device.scss'
})
export class AddDevice {
  message = '';
  serialNumber = '';
  model = '';
  manufacturer = '';
  firmware = '';
  type = -1;
  status = -1;
  location = -1;

  types: any[] = [];
  statuses: any[] = [];
  locations: any[] = [];
}
