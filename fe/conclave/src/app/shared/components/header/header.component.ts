import { Component, OnInit } from '@angular/core';
import { ConclaveAuthService } from '../../../core/authentication/auth.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  isLoggedIn$: Observable<boolean>;                 

  constructor(private authService: ConclaveAuthService) { }

  ngOnInit() {
    this.isLoggedIn$ = this.authService.isLoggedIn;
  }

  get username(){
    return this.authService.username;
  }
  
  onLogout(){
    this.authService.logout();
  }
}
