import { Component } from '@angular/core';
import { TranslationsService } from './services/translation-service';
import { Title } from '@angular/platform-browser';

declare global {
  interface Window { loadingTitle: any; }
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  translations: any;

  showMessage: boolean;
  dontShow: boolean;

  closeMsg() {
    if (this.dontShow)
      localStorage.setItem("hideInformMessage", "true");

    this.showMessage = false;
  }

  constructor(private titleService: Title, private translationService: TranslationsService) {
    this.translationService.getTranslations('Core').subscribe(result => {
      this.translations = result;

      window.clearInterval(window.loadingTitle);
      this.titleService.setTitle(this.translations.pageTitle);
    });

    this.showMessage = localStorage.getItem("hideInformMessage") != 'true';
    this.dontShow = false;
  }
}
