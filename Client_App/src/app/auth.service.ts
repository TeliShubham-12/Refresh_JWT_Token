import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, tap, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly baseUrl = 'http://localhost:5235';

  private readonly JWT_TOKEN = 'JWT_TOKEN';

  constructor(private httpClient: HttpClient, private router: Router) {}

  login(data: any): Observable<boolean> {
    return this.httpClient
      .post<any>(`${this.baseUrl}/users/authenticate`, data,{ withCredentials: true })
      .pipe(
        tap((tokens) => this.doLoginUser(tokens.accessToken)),
        map((res) => {
          console.log(res);
          return res;
        }),
        catchError((error) => {
          alert(error.error);
          return of(false);
        })
      );
  }

  logout() {
    this.doLogoutUser();
  }

  isLoggedIn() {
    return !!this.getJwtToken();
  }

  refreshToken() {
    return this.httpClient
      .post<any>(
        `${this.baseUrl}/users/refresh-token`,
        {
          accessToken: this.getJwtToken(),
        },
        { withCredentials: true }
      )
      .pipe(
        tap((tokens) => {
          this.storeJwtToken(tokens.accessToken);
        }),
        catchError((error) => {
          this.doLogoutUser();
          return of(false);
        })
      );
  }

  getJwtToken() {
    return localStorage.getItem(this.JWT_TOKEN);
  }

  private doLoginUser(token: string) {
    this.storeJwtToken(token);
  }

  private doLogoutUser() {
    this.removeToken();
    this.router.navigate(['/login']);
  }

  private storeJwtToken(jwt: string) {
    localStorage.setItem(this.JWT_TOKEN, jwt);
  }

  private removeToken() {
    localStorage.removeItem(this.JWT_TOKEN);
  }
}
