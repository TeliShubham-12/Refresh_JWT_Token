import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
} from '@angular/common/http';
import { Observable, of, tap } from 'rxjs';

@Injectable()
export class CacheInterceptor implements HttpInterceptor {
  constructor() {
    setInterval(() => this.cleanupExpiredEntries(), 120000); // every two minuts it will clear expired entries
  }
  private cacheExpireTime = 10000; //10 seconds

  private cache: Map<
    string,
    { expireDate: Date; response: HttpResponse<any> }
  > = new Map();

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    if (request.method !== 'GET') {
      return next.handle(request);
    }
    
    const cacheKey = request.url;
    const cachedResponse = this.cache.get(cacheKey);
    const isExpired = cachedResponse && cachedResponse.expireDate <= new Date();

    if (isExpired) {
      this.cache.delete(cacheKey);
      console.log('expired so frpm api');
      return this.sendRequestAndSetCache(request, next);
    }

    if (cachedResponse) {
      console.log('SERVED FROM CACHE');
      return of(cachedResponse.response);
    } else {
      console.log(' not in cache so from api');
      return this.sendRequestAndSetCache(request, next);
    }
  }

  private sendRequestAndSetCache(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      tap((response) => {
        if (response instanceof HttpResponse && response.ok) {
          this.cache.set(request.url, {
            expireDate: new Date(Date.now() + this.cacheExpireTime), // cache expire after 10 seconds
            response: response,
          });
        }
      })
    );
  }

  private cleanupExpiredEntries() {
    this.cache.forEach((value, key) => {
      if (value.expireDate <= new Date()) {
        this.cache.delete(key);
        console.log('Expired key', key);
      }
    });
  }
}
