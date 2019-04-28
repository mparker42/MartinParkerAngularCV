import { Component, Inject } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TranslationsService } from '../services/translation-service';
import { DOCUMENT } from '@angular/platform-browser';

@Component({
  selector: 'navigation-menu',
  templateUrl: './navigation-menu.component.html',
  styleUrls: ['./navigation-menu.component.css']
})
export class NavigationMenuComponent {

  isHandset: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches)
    );

  showSideNav: boolean;

  translations: object;

  constructor(private breakpointObserver: BreakpointObserver, private translationService: TranslationsService, @Inject(DOCUMENT) private document) {
    this.translationService.getTranslations('Navigation').subscribe(result => {
      this.translations = result;
    });
  }

}
