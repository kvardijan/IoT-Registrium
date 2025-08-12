import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserManagerService } from '../user-manager-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-location',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-location.html',
  styleUrl: './add-location.scss'
})
export class AddLocation implements OnInit {
  message = '';
  address = '';
  description = '';
  longitude = 0;
  latitude = 0;

  constructor(private http: HttpClient, public userManager: UserManagerService) { }

  ngOnInit(): void {

  }

  addLocation() {

  }
}
