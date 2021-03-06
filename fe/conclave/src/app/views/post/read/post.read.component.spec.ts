import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PostReadComponent } from './post.read.component';

describe('PostReadComponent', () => {
  let component: PostReadComponent;
  let fixture: ComponentFixture<PostReadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PostReadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostReadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
