import { Component } from '@angular/core';
import { LoginModel } from '../login-model';
import { Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { NgForm } from '@angular/forms';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  invalidLogin: boolean = false;
  credentials: LoginModel = { username: '', password: '' };

  constructor(private router: Router, private authSvc: AuthService) {}

  ngOnInit(): void {}

  login = (form: NgForm) => {
    if (form.valid) {
      this.authSvc.login(this.credentials).subscribe({
        next: (response: any) => {
          const token = response.accessToken;
          if (token != null && token != '') {
            this.router.navigate(['/']);
          } else {
            this.invalidLogin = true;
          }
        },
        error: (err: HttpErrorResponse) => (this.invalidLogin = true),
      });
    }
  };
}
