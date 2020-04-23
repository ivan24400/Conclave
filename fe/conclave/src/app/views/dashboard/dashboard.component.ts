import { Component, OnInit } from '@angular/core';  
import { AuthService } from 'angularx-social-login';  
import { Router } from '@angular/router';
import {PostService} from '../../service/post';
import { ConclaveAuthService } from 'src/app/core/authentication/auth.service';

@Component({  
  selector: 'app-dashboard',  
  templateUrl: './dashboard.component.html',  
  styleUrls: ['./dashboard.component.scss'],
  providers:[PostService]
})  
export class DashboardComponent { 

  public postlist;

  constructor(cauth:ConclaveAuthService, _postService: PostService, private router: Router) {
    let successCb = (result:any)=>{
      this.postlist = result.posts
    };
    let failCb = (err)=>{
      console.log("getfail",err);
      if(err.status == 401){
        cauth.renew(successCb,(err)=>{console.log("dashboardcomponent;",err);});
      }
      this.postlist = [];
    };
    _postService.GetAllPosts(
      successCb,failCb
    );
  }

  onPostAdd(event){
    this.router.navigateByUrl('/dashboard', { skipLocationChange: true }).then(() => {
      this.router.navigate(['DashboardComponent']);
  });   
  }

}