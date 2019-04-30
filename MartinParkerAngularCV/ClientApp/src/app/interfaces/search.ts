export interface ISelectFilterOption {
  value: string;
  translation: string;
}

export interface ISelectFilter {
  name: string;
  titleTranslation: string;
  options: ISelectFilterOption[];
  optionTranslations: any;
}

export interface IDateFilter {
  name: string;
  titleTranslation: string;
  minimumValue: Date;
  maximumValue: Date;
}

export interface IBooleanFilter {
  name: string;
  titleTranslation: string;
}

export interface ISearch {
  selectFilters: ISelectFilter[],
  selectFiltersByName: any,
  dateFilters: IDateFilter[],
  dateFiltersByName: any,
  booleanFilters: IBooleanFilter[],
  booleanFiltersByName: any
}
