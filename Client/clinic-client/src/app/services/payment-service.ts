import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  baseUrl = environment.apiUrl + '/payment';

  constructor(private http: HttpClient) {}

  pay(data: any) {
    return this.http.post(`${this.baseUrl}/pay`, data);
  }

  PatientPayments(id: number) {
    return this.http.get(`${this.baseUrl}/patient/${id}`)
  }

  history() {
    return this.http.get(`${this.baseUrl}/history`);
  }

  
}
