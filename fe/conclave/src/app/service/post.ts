import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { ConclaveAuthService } from '../core/authentication/auth.service';

@Injectable({
    providedIn:'root'
})
export class PostService {

    constructor(private http: HttpClient, private cauth: ConclaveAuthService){}

    GetAllPosts(successCb, failCb){
        const headers = new HttpHeaders()
            .set("Authorization", "Bearer "+this.cauth.accessToken);
        return this.http.get('/api/user/posts',{headers}).subscribe(successCb, failCb)
    }

    AddPost(formData,successCb, failCb){
        const headers = new HttpHeaders()
            .set("Authorization", "Bearer "+this.cauth.accessToken);
        return this.http.post('/api/user/posts',formData,{headers}).subscribe(successCb,failCb);
    }
}