import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthService } from '../../../services/auth-service';

@Component({
  selector: 'app-register',
  imports: [],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {

  roles = ['Patient', 'Doctor'];
  form: FormGroup;

  constructor(private fb: FormBuilder, private auth: AuthService)
  {
      this.form = this.fb.group({
      username: [''],
      email: [''],
      password: [''],
      role: ['Patient'],
      fullName: [''],
      phoneNumber: [''],
      biography: ['']
    });
  }

  isPatient() {
    return this.form.value.role === 'Patient';
  }

  isDoctor() {
    return this.form.value.role === 'Doctor';
  }

  register() {
    this.auth.register(this.form.value).subscribe();
  }



}
