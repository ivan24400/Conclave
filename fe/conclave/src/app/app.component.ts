import { Component, NgZone } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'conclave';
  isHeader: boolean = false;

  constructor(private zone: NgZone, private router: Router) {
    this.router.events.subscribe((event: any) => {
      console.log("URL",event.url);
      if (event instanceof NavigationEnd) {
        if (event.url === '/login' || event.url === '/about' || event.url === '/privacy' || event.url === '/terms') {
          this.isHeader = false;
        } else {
          this.isHeader = true; 
        }
      }
    });
  }
}
