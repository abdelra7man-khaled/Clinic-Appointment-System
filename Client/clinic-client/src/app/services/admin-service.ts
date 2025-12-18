import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl + '/admin';

  constructor(private http: HttpClient){}

  getDoctors() {
    return this.http.get(`${this.baseUrl}/doctors`);
  }

  addDoctor(data: any) {
    return this.http.post(`${this.baseUrl}/add/doctor`, data);
  }

  deleteDoctor(id: number) {
    return this.http.delete(`${this.baseUrl}/delete/doctor/${id}`);
  }

  getSpecialties() {
    return this.http.get(`${this.baseUrl}/specialties`);
  }

  getSpecialty(id: number) {
    return this.http.get(`${this.baseUrl}/specialties/${id}`);
  }
}
