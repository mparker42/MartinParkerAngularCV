import { Component } from '@angular/core';
import { TranslationsService } from '../services/translation-service';

@Component({
    selector: 'about',
    templateUrl: './about.component.html',
    styleUrls: ['./about.component.scss']
})

export class AboutComponent {

  translations: object;
  showAbout: boolean;

  toggleAbout() {
    this.showAbout = !this.showAbout;
  }

  openAbout() {
    this.showAbout = true;
  }

  hideAbout() {
    this.showAbout = false;
  }

  constructor(private translationService: TranslationsService) {
    this.translationService.getTranslations('About').subscribe(result => {
      this.translations = result;
    });

    this.showAbout = false;
  }
}
