import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserManagerService {
  private tokenKey = 'jwtToken';
  isLoggedIn = signal<boolean>(this.hasToken());
  
  constructor(private http: HttpClient){}

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  login(username: string, password: string) {
    return this.http.post<{ token: string }>('http://localhost:5081/api/user', {
      username,
      password
    });
  }

  setToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
    this.isLoggedIn.set(true);
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this.isLoggedIn.set(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
}
