import { Component, OnInit } from '@angular/core';
import {
  AuthService,
  GoogleLoginProvider,
  FacebookLoginProvider
} from "angularx-social-login";

import { UserSocial } from '../../models/UserSocial'
import { ConclaveAuthService } from '../../core/authentication/auth.service';
import { Router, ActivatedRoute, Params } from '@angular/router';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  response;
  errorMessage = 'Login: An error has occured';
  socialuser = new UserSocial();
  
  constructor(
    public OAuth: AuthService,
    private ConclaveAuthService: ConclaveAuthService,
    private router: Router
  ) { }

  ngOnInit() { }

  public socialSignIn(socialProvider: string) {
    // this.router.navigate(['/dashboard']);
    // return;
    let socialPlatformProvider;
    if (socialProvider === 'facebook') {
      socialPlatformProvider = FacebookLoginProvider.PROVIDER_ID;
    } else if (socialProvider === 'google') {
      socialPlatformProvider = GoogleLoginProvider.PROVIDER_ID;
    }
    this.OAuth.signIn(socialPlatformProvider).then(socialuser => {
      this.ServerLogin(socialuser);
    });
  }

  ServerLogin(socialuser: any) {
    this.ConclaveAuthService.login(socialuser,'/dashboard');
  }
}