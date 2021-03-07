import { Component, Output, EventEmitter } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { IPortfolioTile } from '../../interfaces/portfolio';
import { TranslationsService } from '../../services/translation-service';

@Component({
  selector: 'search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})

export class SearchComponent {
  @Output() searchTermChanged: EventEmitter<string> = new EventEmitter<string>();

  translations: any;
  private _searchTerm: string;

  private debouncer: Subject<string> = new Subject<string>();

  get searchTerm(): string {
    return this._searchTerm;
  };
  set searchTerm(st: string) {
    this._searchTerm = st;
    this.debouncer.next(st);
  }

  searchTiles(tiles: IPortfolioTile[]): IPortfolioTile[] {
    if(!this.searchTerm)
      return tiles;

    let filteredTiles = [];
    tiles.forEach((tile: IPortfolioTile): void => {
      let isInSearch: Boolean = false;
      this.searchTerm.toLowerCase().split(' ').forEach((st: string): void => {
        isInSearch = isInSearch || this.translations[tile.searchCriteriaTranslation].indexOf(st) > -1;
      });

      if(isInSearch)
        filteredTiles.push(tile);
    });

    return filteredTiles;
  }

  constructor(private translationService: TranslationsService) {
    this.translationService.getTranslations('Search').subscribe(result => {
      this.translations = result;
    });

    this.debouncer.pipe(debounceTime(500)).subscribe((st: string): void => { this.searchTermChanged.emit(st); });
  }
}
