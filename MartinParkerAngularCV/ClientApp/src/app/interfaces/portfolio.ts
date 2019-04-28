import { ISearch } from '../interfaces/search';

export interface IPortfolioTag {
  name: string;
  selectValue: string;
  dateValue: Date;
  booleanValue: boolean;
}

export interface IPortfolioTile {
  titleTranslation: string;
  descriptionTranslation: string;
  link: string;
  searchCriteriaTranslation: string;
  imageURL: string;
  imageAltTextTranslation: string;
  tags: IPortfolioTag[];
}

export interface IPortfolio {
  tiles: IPortfolioTile[];
  search: ISearch;
}
