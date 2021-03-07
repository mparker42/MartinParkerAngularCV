import { Component, Output, EventEmitter } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { IBooleanFilter, IDateFilter, IFilter, ISearch, ISelectFilter, ISelectFilterOption } from '../../interfaces/search';
import { PortfolioService } from '../../services/portfolio-service';
import { IPortfolioTag, IPortfolioTile } from '../../interfaces/portfolio';
import { TranslationsService } from '../../services/translation-service';
import { MatSelect } from '@angular/material/select';

@Component({
  selector: 'filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss']
})

export class FilterComponent {
  @Output() filterChanged: EventEmitter<ISearch> = new EventEmitter<ISearch>();

  translations: any;
  availableFilters: { [id: string]: IFilter } = {};
  availableFilterNames: string[] = [];
  newFilterPicker: MatSelect;
  selectedFilterType: string;
  booleanFilterSelection: boolean;
  selectFilterSelection: ISelectFilterOption;

  private _chosenFilters: ISearch = {
    selectFilters: [],
    selectFiltersByName: null,
    booleanFilters: [],
    booleanFiltersByName: null,
    dateFilters: [],
    dateFiltersByName: null
  };
  private _newFilter: IFilter;
  private debouncer: Subject<ISearch> = new Subject<ISearch>();

  get chosenFilters(): ISearch {
    return this._chosenFilters;
  };
  set chosenFilters(st: ISearch) {
    this._chosenFilters = st;
    this.debouncer.next(st);
  }

  get newFilter(): IFilter {
    return this._newFilter;
  }
  set newFilter(nf: IFilter) {
    this._newFilter = nf;

    this.newFilterPickerChanged(nf);
  }

  newFilterPicked() {
    switch(this.selectedFilterType) {
      case 'boolean':
        const booleanFilter = this._newFilter as IBooleanFilter;
        booleanFilter.value = this.booleanFilterSelection;

        this.chosenFilters.booleanFilters.push(booleanFilter);

        this.booleanFilterSelection = null;
        break;
      case 'select':
        const selectFilter = this._newFilter as ISelectFilter;
        selectFilter.value = this.selectFilterSelection;

        this.chosenFilters.selectFilters.push(selectFilter);

        this.selectFilterSelection = null;
        break;
      case 'date':
        break;
    }

    const filters: string[] = [];
    Object.getOwnPropertyNames(this.availableFilters).forEach(f => {
      if(f !== this.newFilter.name)
        filters.push(f);
    });
    this.availableFilterNames = filters.sort((a: string, b: string): number => b.localeCompare(a));
    this.selectedFilterType = null;
    this.newFilter = null;

    this.debouncer.next(this.chosenFilters);
  }

  areAnyFilters(){
    return this.chosenFilters && (
      this.chosenFilters.booleanFilters?.length ||
      this.chosenFilters.dateFilters?.length ||
      this.chosenFilters.selectFilters?.length
    );
  }

  removeBooleanFilter(booleanFilter: IBooleanFilter): void {
    let newFilters: IBooleanFilter[] = [];

    this.chosenFilters.booleanFilters.forEach((bf: IBooleanFilter): void => {
      if(booleanFilter.name != bf.name)
        newFilters.push(bf);
    });

    this.chosenFilters.booleanFilters = newFilters;
    this.availableFilterNames.push(booleanFilter.name);
    this.availableFilterNames = Object.getOwnPropertyNames(this.availableFilters).sort((a: string, b: string): number => b.localeCompare(a));
    this.debouncer.next(this.chosenFilters);
  }

  removeSelectFilter(selectFilter: ISelectFilter): void {
    let newFilters: ISelectFilter[] = [];

    this.chosenFilters.selectFilters.forEach((sf: ISelectFilter): void => {
      if(selectFilter.name != sf.name)
        newFilters.push(sf);
    });

    this.chosenFilters.selectFilters = newFilters;
    this.availableFilterNames.push(selectFilter.name);
    this.availableFilterNames = Object.getOwnPropertyNames(this.availableFilters).sort((a: string, b: string): number => b.localeCompare(a));
    this.debouncer.next(this.chosenFilters);
  }

  removeDateFilter(dateFilter: IDateFilter): void {
    let newFilters: IDateFilter[] = [];

    this.chosenFilters.dateFilters.forEach((df: IDateFilter): void => {
      if(dateFilter.name != df.name)
        newFilters.push(df);
    });

    this.chosenFilters.dateFilters = newFilters;
    this.availableFilterNames.push(dateFilter.name);
    this.availableFilterNames = Object.getOwnPropertyNames(this.availableFilters).sort((a: string, b: string): number => b.localeCompare(a));
    this.debouncer.next(this.chosenFilters);
  }

  filterTiles(tiles: IPortfolioTile[]): IPortfolioTile[] {
    if(!this.areAnyFilters())
      return tiles;

    let filteredTiles = [];
    tiles.forEach((tile: IPortfolioTile): void => {
      let isInFilter: Boolean = true;
      this.chosenFilters.booleanFilters.forEach((bf: IBooleanFilter): void => {
        if(!isInFilter)
          return;

        let foundTag: IPortfolioTag = null

        tile.tags.forEach((t: IPortfolioTag): void => {
          if(t.name == bf.name)
            foundTag = t;
        });
        isInFilter = (!bf.value && !(foundTag?.booleanValue ?? false)) || (bf.value && (foundTag?.booleanValue ?? false));
      });
      this.chosenFilters.selectFilters.forEach((sf: ISelectFilter): void => {
        if(!isInFilter)
          return;

        let foundTag: IPortfolioTag = null;

        tile.tags.forEach((t: IPortfolioTag): void => {
          if(t.name == sf.name)
            foundTag = t;
        })

        isInFilter = foundTag?.selectValue === sf.value.value;
      });

      if(isInFilter)
        filteredTiles.push(tile);
    });

    return filteredTiles;
  }

  private newFilterPickerChanged(filter: IFilter): void {
    if(!filter)
      return;

    const booleanFilter: IBooleanFilter = filter as IBooleanFilter;
    const selectFilter: ISelectFilter = filter as ISelectFilter;
    const dateFilter: IDateFilter = filter as IDateFilter;

    if(dateFilter?.minimumValue) {
      this.selectedFilterType = 'date';
    } else if(selectFilter?.optionTranslations) {
      this.selectedFilterType = 'select';
    } else {
      this.selectedFilterType = 'boolean';
    }
  }

  constructor(private translationService: TranslationsService, private portfolioService: PortfolioService) {
    this.translationService.getTranslations('Filter').subscribe(result => {
      this.translations = result;
    });

    this.portfolioService.getSearchFilters().subscribe((result: ISearch): void => {
      this.availableFilters = {};

      result.booleanFilters.forEach((f: IFilter) => { this.availableFilters[f.name] = f; });
      result.dateFilters.forEach((f: IFilter) => { this.availableFilters[f.name] = f; });
      result.selectFilters.forEach((f: IFilter) => { this.availableFilters[f.name] = f; });

      this.availableFilterNames = Object.getOwnPropertyNames(this.availableFilters).sort((a: string, b: string): number => b.localeCompare(a));
    });

    this.debouncer.pipe(debounceTime(500)).subscribe((st: ISearch): void => { this.filterChanged.emit(st); });
  }
}
