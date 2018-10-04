import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

import { LogService } from './_services/log.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent  implements OnInit {
  title = 'FAB';

  constructor(
    router: Router,
    private logService: LogService
  ) {

    router.events.subscribe(s => {
      if (s instanceof NavigationEnd) {
        const tree = router.parseUrl(router.url);
        if (tree.fragment) {
          const element = document.querySelector('#' + tree.fragment);
          if (element) { element.scrollIntoView(true); }
        }
      }
    });
  }


  ngOnInit() {
      this.logService.createLog().subscribe( () => {
        console.log('app.component.ts->ngOnInit()->log created.');
      }, error => {
        console.log('app.component.ts->ngOnInit()->log already exist or log creation error: ' + error);
      }, () => {
        // once saved, stop showing busy loading animation icon
        console.log('no errors encountered while creating log.');
      });
  }

}
