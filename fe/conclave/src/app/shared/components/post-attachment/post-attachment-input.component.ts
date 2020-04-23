import {Component, Input,Output,ViewChild, ElementRef, Renderer2,AfterViewInit} from '@angular/core';
import { EventEmitter } from 'protractor';

@Component({
    selector:'post-attachment-input',
    template: `<div [class.d-none]="!displayComponent">
    <label class='post-attachment-name'>{{post_attachment_name}}</label>
    <input #postfileinput name='{{inputName}}' class="post-attached-input" [class.d-none]="inputType != 'text'" type='{{inputType}}' on-change='setLabelOnAttached($event)' accept='{{acceptFormat}}' />
    <label title="Delete" (click)='remove()' style="margin:0 5px">🗑</label>
    </div>`
})
export class PostAttachmentInput{
    @Input() inputName:string;
    @Input() inputType:string;
    @Input() acceptFormat:string;

    _ref:any;
    post_attachment_name:any;
    displayComponent:boolean = false

    @ViewChild('postfileinput') postFileInput:ElementRef;

    constructor(){}

    showFileDialog(){
        this.postFileInput.nativeElement.click();
    }

    setLabelOnAttached(event){
        console.log("inputfile-onchange:",event);
        this.displayComponent = true;
        this.post_attachment_name = event.currentTarget.files[0].name
    }

    remove() {
        this._ref.destroy();
    }   
}