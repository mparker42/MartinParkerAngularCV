import { Component } from '@angular/core';
import { TranslationsService } from '../services/translation-service';
import { PortfolioService } from '../services/portfolio-service';
import { IPortfolio, IPortfolioTag } from '../interfaces/portfolio';
import { forEach } from '@angular/router/src/utils/collection';
import { open } from 'inspector';

@Component({
  selector: 'portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.scss']
})

export class PortfolioComponent {
  translations: any;
  portfolio: IPortfolio = null;

  getLanguageTag(tags: IPortfolioTag[]) {
    for (let i = 0; i < tags.length; i++) {
      let tag = tags[i];

      if (tag.name === "Language")
        return tag;
    }

    return null;
  }

  getTranslationForSelectOption(tag: IPortfolioTag) {
    for (let i = 0; i < this.portfolio.search.selectFilters.length; i++) {
      let filter = this.portfolio.search.selectFilters[i];

      if (filter.name === tag.name) {
        for (let j = 0; j < filter.options.length; j++) {
          let option = filter.options[j];

          if (option.value === tag.selectValue) {
            return option.translation;
          }
        }

        return null;
      }
    }

    return null;
  }

  constructor(private translationService: TranslationsService, private portfolioService: PortfolioService) {
    this.translationService.getTranslations('Portfolio').subscribe(result => {
      this.translations = result;
    });

    this.portfolioService.getTileData().subscribe(result => {
      this.portfolio = result;
    });
  }
}
