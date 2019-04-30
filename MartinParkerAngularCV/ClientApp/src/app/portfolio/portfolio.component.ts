import { Component } from '@angular/core';
import { TranslationsService } from '../services/translation-service';
import { PortfolioService } from '../services/portfolio-service';
import { IPortfolio, IPortfolioTag } from '../interfaces/portfolio';

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
    return this.portfolio.search.selectFiltersByName[tag.name].optionTranslations[tag.selectValue];
  }

  getTagChip(tag: IPortfolioTag) {
    let tagFormat = this.translations["tagFormat"];

    if (tag.selectValue !== undefined) {
      let selectFilter = this.portfolio.search.selectFiltersByName[tag.name];

      return tagFormat.replace(/\{0\}/gi, this.translations[selectFilter.titleTranslation]).replace(/\{1\}/gi, this.translations[selectFilter.optionTranslations[tag.selectValue]]);
    }

    if (tag.dateValue !== undefined) {
      let dateFilter = this.portfolio.search.dateFiltersByName[tag.name];

      return tagFormat.replace(/\{0\}/gi, this.translations[dateFilter.titleTranslation]).replace(/\{1\}/gi, tag.dateValue);
    }

    let booleanFilter = this.portfolio.search.booleanFiltersByName[tag.name],
      booleanTranslation = this.translations[booleanFilter.titleTranslation];

    if (tag.booleanValue)
      return booleanTranslation;

    return this.translations['notBooleanFormat'].replace(/\{0\}/gi, booleanTranslation);
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
