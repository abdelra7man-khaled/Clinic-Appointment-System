import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AppointmentService {
  baseUrl = environment.apiUrl + '/appointment';

  constructor(private http: HttpClient) {}

  book(data: any) {
    return this.http.post(`${this.baseUrl}/appointments`, data);
  }

  DoctorSchedules(id: number, dateFrom?: Date, dateTo?: Date) {
    let params = new HttpParams();

    if (dateFrom) {
      params = params.set('dateFrom', dateFrom.toISOString());
    }

    if (dateTo) {
      params = params.set('dateTo', dateTo.toISOString());
    }

    return this.http.get(
      `${this.baseUrl}/doctor/${id}/schedule`,
      { params }
    );
  }

  confirm(id: number){
    return this.http.post(`${this.baseUrl}/${id}/confirm`,{})
  }
  cancel(id: number) {
    return this.http.post(`${this.baseUrl}/appointments/${id}/cancel`, {});
  }
  delete(id: number) {
    return this.http.delete(`${this.baseUrl}/appointments/${id}/delete`);
  }
}
