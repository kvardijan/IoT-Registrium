import { Component, signal } from '@angular/core';
import { RouterOutlet, Router, RouterLink } from '@angular/router';
import { UserManagerService } from './user-manager-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('IotRegistryFrontend');

  constructor(private router: Router, public userManager: UserManagerService) { }

  onLoginClick() {
    if (this.userManager.isLoggedIn()) {
      this.userManager.logout();
    } else {
      this.router.navigate(['/login']);
    }
  }
}
