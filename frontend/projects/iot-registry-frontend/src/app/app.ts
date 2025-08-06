import { Component, signal } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('IotRegistryFrontend');

  constructor(private router: Router) { }

  onLoginClick() {
    this.router.navigate(['/login']);
  }
}
