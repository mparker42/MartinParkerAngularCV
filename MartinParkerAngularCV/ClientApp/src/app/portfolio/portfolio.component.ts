import { Component } from '@angular/core';
import { TranslationsService } from '../services/translation-service';
import { PortfolioService } from '../services/portfolio-service';
import { IPortfolio } from '../interfaces/portfolio';

@Component({
  selector: 'portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.scss']
})

export class PortfolioComponent {
  translations: any;
  portfolio: IPortfolio = null;

  constructor(private translationService: TranslationsService, private portfolioService: PortfolioService) {
    this.translationService.getTranslations('Portfolio').subscribe(result => {
      this.translations = result;
    });

    this.portfolioService.getTileData().subscribe(result => {
      this.portfolio = result;
    });
  }
}
