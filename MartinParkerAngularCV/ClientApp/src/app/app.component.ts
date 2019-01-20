import { Component } from '@angular/core';
import { TranslationsService } from './services/translation-service';
import { Title } from '@angular/platform-browser';

declare global {
  interface Window { loadingTitle: any; }
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  translations: any;

  constructor(private titleService: Title, private translationService: TranslationsService) {
    this.translationService.getTranslations('Core').subscribe(result => {
      this.translations = result;

      window.clearInterval(window.loadingTitle);
      this.titleService.setTitle(this.translations.pageTitle);
    });
  }
}
