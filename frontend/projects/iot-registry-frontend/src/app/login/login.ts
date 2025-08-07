import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserManagerService } from '../user-manager-service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {
  username = '';
  password = '';
  message = '';

  constructor(private userManager: UserManagerService, private router: Router) {

  }

  ngOnInit(): void {

  }

  login() {
    this.message = '';
    this.userManager.login(this.username, this.password).subscribe({
      next: (response) => {
        if (response.success) {
          this.userManager.setToken(response.data);
          this.router.navigate(['/']);
        } else {
          this.message = 'Login failed. Please check credentials.';
          console.error('Login failed: ', response.error);
        }
      },
      error: (err) => {
        this.message = 'Login failed. Please check credentials.';
        console.error('Login failed', err);
      }
    });
  }
}
