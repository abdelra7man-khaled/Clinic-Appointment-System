import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})

export class AppointmentService {
  baseUrl = environment.apiUrl + '/appointment';

  constructor(private http: HttpClient) {}

  book(data: any) {
    return this.http.post(`${this.baseUrl}/appointments`, data);
  }

  DoctorSchedules(id: number , dateFrom?: Date , dateTo?: Date) {
    return this.http.get(`${this.baseUrl}/doctor/${id}/schedule` , dateFrom , dateTo);
  }

  cancel(id: number) {
    return this.http.post(`${this.baseUrl}/appointments/${id}/cancel`, {});
  }
}
