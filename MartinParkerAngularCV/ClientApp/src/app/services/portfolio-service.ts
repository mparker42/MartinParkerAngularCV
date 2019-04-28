import { IPortfolio } from '../interfaces/portfolio';
import { ISearch } from '../interfaces/search';
import { PublicBlobFilesService } from './public-blob-files-service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

// Make this a singleton for the locale resolution.
@Injectable({
  providedIn: 'root'
})
export class PortfolioService {
  private portfolioData: IPortfolio = null;
  private searchData: ISearch = null;

  getTileData() {
    return new Observable<IPortfolio>(observer => {
      let portfolioObservable = this.blobService.get<IPortfolio>('DataFiles/V1/TileDefinition.json'),
        searchObservable = this.blobService.get<ISearch>('DataFiles/V1/TileSearch.json');

      portfolioObservable.then(result => {
        this.portfolioData = result;

        if (this.searchData != null) {
          this.portfolioData.search = this.searchData;

          observer.next(this.portfolioData);
          observer.complete();
        }
      });

      searchObservable.then(result => {
        this.searchData = result;

        if (this.portfolioData != null) {
          this.portfolioData.search = this.searchData;

          observer.next(this.portfolioData);
          observer.complete();
        }
      });
    });
  }

  constructor(private blobService: PublicBlobFilesService) {
  }
}
