import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthService } from '../../../services/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

   form: FormGroup;

    constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
      this.form = this.fb.group({
        username: [''],
        password: ['']
      });
    }

    login() {
    this.auth.login(this.form.value).subscribe(res => {
      this.auth.saveToken(res.token);
      this.router.navigate(['/']);
    });
  }

}
