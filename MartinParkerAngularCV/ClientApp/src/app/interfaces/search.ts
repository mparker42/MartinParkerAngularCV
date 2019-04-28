export interface ISelectFilterOption {
  value: string;
  translation: string;
}

export interface ISelectFilter {
  name: string;
  titleTranslation: string;
  options: ISelectFilterOption[]
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
  dateFilters: IDateFilter[],
  booleanFilters: IBooleanFilter[]
}
