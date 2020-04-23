import { Component, OnInit,ViewChild,ViewContainerRef,ComponentFactoryResolver, Output,EventEmitter } from '@angular/core';
import {PostService} from '../../../service/post';
import {PostAttachmentInput} from '../../../shared/components/post-attachment/post-attachment-input.component';

@Component({
  selector: 'app-post-add',
  templateUrl: './post.add.component.html',
  styleUrls: ['./post.add.component.scss'],
  providers:[PostService]
})
export class PostAddComponent implements OnInit {
  post_add_error : string = "";
  @Output() postAddSuccess : EventEmitter<any> = new EventEmitter();

  items;
  avAttachmentCount = 0;
  imgAttachmentCount = 0;
  embedAttachmentCount = 0;

  componentRef: any;

  @ViewChild('attachment_container', {static:true, read: ViewContainerRef }) attachmentList: ViewContainerRef;

  constructor(
    private _cfr: ComponentFactoryResolver,
    private _addPostService : PostService
  ) {  }

  ngOnInit() {
  }

  onPostSubmit(postData) {
    debugger;
    this._addPostService.AddPost(
      postData,
      (res)=>{
        this.postAddSuccess.emit(null);
      },
      (err)=>{
        this.post_add_error="Post could not be added";
      })
      ;
  }

  createAttachmentComponent(inputName, inputType, acceptFormat) {
    const factory = this._cfr.resolveComponentFactory(PostAttachmentInput);
    var componentRef = this.attachmentList.createComponent(factory);
    componentRef.instance.inputType = inputType;
    componentRef.instance.acceptFormat = acceptFormat;
    componentRef.instance.inputName = inputName;
    componentRef.instance._ref = componentRef;
    return componentRef;
}


  addAttachment(filetype:string){
    let attachedComponent;
    switch(filetype){
      case 'av':
        attachedComponent = this.createAttachmentComponent(filetype,"file","audio/*,video/*");
        break;
      case 'img':
        attachedComponent = this.createAttachmentComponent(filetype,"file","image/*");
        break;
      case 'embed':
        attachedComponent = this.createAttachmentComponent(filetype,"text","");
          break;
    }
    attachedComponent.changeDetectorRef.detectChanges();
    attachedComponent.instance.showFileDialog();
  }
}