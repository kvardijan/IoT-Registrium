import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { UserManagerService } from '../user-manager-service';
import { Router, RouterModule } from '@angular/router';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-locations-table',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './locations-table.html',
  styleUrl: './locations-table.scss'
})
export class LocationsTable implements OnInit {
  locations: any[] = [];

  constructor(private http: HttpClient, private router: Router, public userManager: UserManagerService) {

  }

  ngOnInit(): void {
    this.fetchLocations();
  }

  fetchLocations() {
    this.http.get<any>(environment.locationApi)
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

  onAddLocationClick() {
    if (this.userManager.isLoggedIn()) {
      this.router.navigate(['/addlocation']);
    } else {
      this.router.navigate(['/login']);
    }
  }
}
