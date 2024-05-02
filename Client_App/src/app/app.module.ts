import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
// import { CacheInterceptor } from './cache.interceptor';
import { tokenInterceptor } from './token.interceptor';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { FormsModule } from '@angular/forms';
import { JwtModule } from '@auth0/angular-jwt';
export function tokenGetter() { 
  return localStorage.getItem("JWT_TOKEN"); 
}
@NgModule({
  declarations: [
    AppComponent, 
    LoginComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ["localhost:5253"],
        disallowedRoutes: []
      }
    })
  ],
  // providers: [
  //   {
  //     provide:HTTP_INTERCEPTORS,
  //     useClass:CacheInterceptor,
  //     multi:true
  //   }
  // ],
  providers: [
    tokenInterceptor
  ],

  bootstrap: [AppComponent]
})
export class AppModule { }
