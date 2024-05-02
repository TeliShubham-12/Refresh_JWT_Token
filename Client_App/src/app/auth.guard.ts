import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router, private jwtHelper: JwtHelperService) {}

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const token = localStorage.getItem('JWT_TOKEN');

    if (token && !this.jwtHelper.isTokenExpired(token)) {
      console.log(this.jwtHelper.decodeToken(token));
      return true;
    }

    this.router.navigate(['login']);
    return false;
  }
}
