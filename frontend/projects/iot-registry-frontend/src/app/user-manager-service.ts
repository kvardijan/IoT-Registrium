import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from './environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserManagerService {
  private tokenKey = 'jwtToken';
  isLoggedIn = signal<boolean>(this.hasToken());
  username = signal<string>(this.extractUsername());

  constructor(private http: HttpClient) { }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  private extractUsername(): string {
    const token = this.getToken();
    if (!token) return 'Guest';
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.name || 'Guest';
    } catch (e) {
      return 'Guest';
    }
  }

  login(username: string, password: string) {
    return this.http.post<{ success: boolean; data: string; error: string }>(environment.userApi + '/api/user/login', {
      username,
      password
    });
  }

  setToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
    this.isLoggedIn.set(true);
    this.username.set(this.extractUsername());
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this.isLoggedIn.set(false);
    this.username.set('Guest');
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
}
