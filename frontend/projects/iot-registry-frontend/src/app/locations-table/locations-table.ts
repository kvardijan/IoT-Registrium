import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-locations-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './locations-table.html',
  styleUrl: './locations-table.scss'
})
export class LocationsTable implements OnInit {
  locations: any[] = [];

  constructor(private http: HttpClient) {

  }

  ngOnInit(): void {
    this.fetchLocations();
  }

    fetchLocations() {
    this.http.get<any>('http://localhost:5261/api/location')
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.locations = response.data;
          } else {
            console.error('Failed to fetch locations:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching locations', err);
        }
      });
  }
}
