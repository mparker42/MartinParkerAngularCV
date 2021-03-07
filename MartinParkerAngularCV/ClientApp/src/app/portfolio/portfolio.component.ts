import { Component, ViewChild } from '@angular/core';
import { TranslationsService } from '../services/translation-service';
import { PortfolioService } from '../services/portfolio-service';
import { IPortfolio, IPortfolioTag, IPortfolioTile } from '../interfaces/portfolio';
import { SearchComponent } from './search/search.component';
import { FilterComponent } from './filter/filter.component';

@Component({
  selector: 'portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.scss']
})

export class PortfolioComponent {
  @ViewChild('portfolioSearch')search: SearchComponent;
  @ViewChild('portfolioFilter')filter: FilterComponent;
  translations: any;
  portfolio: IPortfolio = null;
  filteredTiles: IPortfolioTile[] = [];

  getLanguageTag(tags: IPortfolioTag[]): IPortfolioTag {
    for (let i = 0; i < tags.length; i++) {
      let tag = tags[i];

      if (tag.name === "Language")
        return tag;
    }

    return null;
  }

  getTranslationForSelectOption(tag: IPortfolioTag): string {
    return this.portfolio.search.selectFiltersByName[tag.name].optionTranslations[tag.selectValue];
  }

  getTagChip(tag: IPortfolioTag): string {
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

  filterTiles(): void {
    if(!this.filter || !this.filter.filterTiles ||
      !this.search || !this.search.searchTiles) {
      setTimeout(() => this.filterTiles(), 500);
      return;
    }

    this.filteredTiles =
      this.search.searchTiles(
        this.filter.filterTiles(this.portfolio?.tiles)
      );
  }

  constructor(private translationService: TranslationsService, private portfolioService: PortfolioService) {
    this.translationService.getTranslations('Portfolio').subscribe(result => {
      this.translations = result;
    });

    this.portfolioService.getTileData().subscribe(result => {
      this.portfolio = result;
      this.filterTiles();
    });
  }
}
