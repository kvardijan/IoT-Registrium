import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserManagerService } from '../user-manager-service';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-statistic',
  standalone: true,
  imports: [],
  templateUrl: './statistic.html',
  styleUrl: './statistic.scss'
})
export class Statistic implements OnInit {
  active = 0;
  idle = 0;
  deactivated = 0;
  error = 0;
  total = 0;
  activePercent = 0;
  idlePercent = 0;
  deactivatedPercent = 0;
  errorPercent = 0;
  totalPercent = 0;

  constructor(private http: HttpClient, public userManager: UserManagerService) { }

  ngOnInit(): void {
    this.fetchStatusStatistic();
  }

  fetchStatusStatistic() {
    const jwt = this.userManager.getToken();
    const headers = {
      Authorization: 'Bearer ' + jwt
    };

    this.http.get<any>(environment.statisticApi, { headers })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.active = response.data.active;
            this.idle = response.data.idle;
            this.deactivated = response.data.deactivated;
            this.error = response.data.error;
            this.total = response.data.active + response.data.idle + response.data.deactivated + response.data.error;

            this.activePercent = Math.round((this.active / this.total) * 10000) / 100;
            this.idlePercent = Math.round((this.idle / this.total) * 10000) / 100;
            this.deactivatedPercent = Math.round((this.deactivated / this.total) * 10000) / 100;
            this.errorPercent = Math.round((this.error / this.total) * 10000) / 100;
          } else {
            console.error('Failed to fetch statuses:', response.error);
          }
        },
        error: (err) => {
          console.error('Error fetching statuses', err);
        }
      });
  }
}
