import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-edit-device',
  standalone: true,
  imports: [FormsModule, MatSelectModule, MatFormFieldModule],
  templateUrl: './edit-device.html',
  styleUrl: './edit-device.scss'
})
export class EditDevice {
  message = '';
  model = '';
  manufacturer = '';
  firmware = '';
  type = -1;
  status = -1;
  location = -1;

  types: any[] = [];
  statuses: any[] = [];
  locations: any[] = [];

  editDevice(){
    
  }
}
