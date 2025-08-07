import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UserManagerService } from '../user-manager-service';
@Component({
  selector: 'app-device-events',
  standalone: true,
  imports: [],
  templateUrl: './device-events.html',
  styleUrl: './device-events.scss'
})
export class DeviceEvents implements OnInit {
  serialNumber: string = '';
  events: any[] = [];

  constructor(private route: ActivatedRoute, private http: HttpClient, public userManager: UserManagerService) { }

  ngOnInit(): void {
    this.serialNumber = this.route.snapshot.paramMap.get('serialNumber') || '';
    this.fetchEvents();
  }

  fetchEvents() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };

    this.http.get<any>('http://localhost:5282/api/event/device/' + this.serialNumber, { headers })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.events = response.data;
            console.log(this.events);
          } else {
            console.error('Failed to fetch events:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching events', err);
        }
      });
  }
}
