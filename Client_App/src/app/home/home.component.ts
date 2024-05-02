import { Component, OnInit } from '@angular/core';
import { User } from '../user';
import { UserService } from '../user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  users: User[] = [];
  requestCount: number = 0;

  constructor(private userSvc: UserService) {}

  ngOnInit(): void {
  
    setInterval(() => {
      this.userSvc.getUsers().subscribe((res) => {
        this.users = res;
        this.requestCount++;
      });
    }, 10000);
  }
}
