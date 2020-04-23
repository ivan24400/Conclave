import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SocialLoginModule, AuthServiceConfig } from "angularx-social-login";
import { GoogleLoginProvider, FacebookLoginProvider } from "angularx-social-login";

import { AppComponent } from './app.component';
import { LoginComponent } from './views/login/login.component';
import { DashboardComponent } from './views/dashboard/dashboard.component';
import { AppRoutingModule } from './routes/app-routing.module';

import {ConclaveAuthService} from './core/authentication/auth.service';
import {AuthGuard} from './core/authentication/auth.guard';

import {SocialCred} from './config/social';
import { HeaderComponent } from './shared/components/header/header.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { PostAddComponent } from './views/post/add/post.add.component';
import { PostReadComponent } from './views/post/read/post.read.component';
import {PostAttachmentInput} from './shared/components/post-attachment/post-attachment-input.component';
import { TosComponent } from './views/tos/tos.component';
import { AboutComponent } from './views/about/about.component';
import { PrivacyComponent } from './views/privacy/privacy.component';

export function socialConfigs() {

  const config = new AuthServiceConfig(
    [
      {
        id: FacebookLoginProvider.PROVIDER_ID,
        provider: new FacebookLoginProvider(SocialCred.Facebook.APP_ID)
      },
      {
        id: GoogleLoginProvider.PROVIDER_ID,
        provider: new GoogleLoginProvider(SocialCred.Google.OAUTH_CLIENT_ID)
      }
    ]
  );
  return config;
}

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    HeaderComponent,
    FooterComponent,
    PostAddComponent,
    PostReadComponent,
    TosComponent,
    AboutComponent,
    PrivacyComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    SocialLoginModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    { 
      provide: AuthServiceConfig,  
      useFactory: socialConfigs  
    },
    ConclaveAuthService,
    AuthGuard
  ],
  bootstrap: [AppComponent],
  entryComponents:[PostAttachmentInput]
})
export class AppModule { }
