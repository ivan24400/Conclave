import { Component, OnInit,Input } from '@angular/core';


@Component({
  selector: 'app-post-read',
  templateUrl: './post.read.component.html',
  styleUrls: ['./post.read.component.scss']
})
export class PostReadComponent implements OnInit {
  @Input() post: any

  constructor(  ) {}

  ngOnInit() {}

}