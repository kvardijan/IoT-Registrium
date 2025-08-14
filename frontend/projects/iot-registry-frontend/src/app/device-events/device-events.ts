import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { UserManagerService } from '../user-manager-service';
import { EditDevice } from '../edit-device/edit-device';
import { environment } from '../environments/environment';
@Component({
  selector: 'app-device-events',
  standalone: true,
  imports: [DatePipe, EditDevice],
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

    this.http.get<any>(environment.eventApi + '/api/event/device/' + this.serialNumber, { headers })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.events = response.data.sort((a: any, b: any) =>
              new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
            );
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
