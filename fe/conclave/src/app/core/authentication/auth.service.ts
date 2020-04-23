import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

import { BehaviorSubject } from 'rxjs';


@Injectable()
export class ConclaveAuthService {
  private loggedIn = new BehaviorSubject<boolean>(false);
  
  private _email:string;
  private _accessToken:string;
  private _refreshToken:string;

  get isLoggedIn() {
    return this.loggedIn.asObservable();
  }

  get username(){
    return this._email != undefined ? this._email.split('@')[0] : "user";
  }

  get accessToken(){
    return this._accessToken;
  }

  get refreshToken(){
    return this._refreshToken;
  }

  constructor(
    private router: Router,
    private http: HttpClient
  ) { }

  login(data, url) {
    this.http.post('/api/access/login', data).subscribe(
      (result:any) => {
        this.loggedIn.next(true);
        this._email = result.email;
        this._accessToken = result.accessToken;
        this._refreshToken = result.refreshToken;
        console.log("TOKEN",result);
      }, error => {
        console.log("authservice:login:f:", error);
      },
      () => {
        this.router.navigate([url]);
      }
    );
  }

  renew(successCb,failCb){
    const reqData = {accessToken:this._accessToken, refreshToken:this._refreshToken};

    return this.http.post<any>('/api/access/renew',reqData).subscribe(
      (result:any) => {
        if(result.success == true){
          this._accessToken = result.accessToken;
        }
        successCb();
      },
      (err)=>{
        failCb(err);
      }
    );
  }

  logout() {
    this.loggedIn.next(false);
    this.router.navigate(['/login']);
  }
}