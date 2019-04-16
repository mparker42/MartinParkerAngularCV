import { Component } from '@angular/core';
import { TranslationsService } from '../services/translation-service';

@Component({
  selector: 'portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.scss']
})

export class PortfolioComponent {

  translations: object;


  constructor(private translationService: TranslationsService) {
    this.translationService.getTranslations('Portfolio').subscribe(result => {
      this.translations = result;
    });

  }
}
