import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})

export class AuthService
{
    private baseUrl = environment.apiUrl + '/auth';

    constructor(private http: HttpClient) {}

    login(data: any) {
      return this.http.post<any>(`${this.baseUrl}/login`, data);
    }

    register(data: any) {
      return this.http.post(`${this.baseUrl}/register`, data);
    }

    saveToken(token: string) {
      localStorage.setItem('token', token);
    }

    getToken() {
      return localStorage.getItem('token');
    }

    logout() {
      localStorage.clear();
    }
}
