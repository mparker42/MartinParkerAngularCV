import { ITranslations } from '../interfaces/translations';
import { APIRequestService } from './api-request-service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

// Make this a singleton for the locale resolution.
@Injectable({
  providedIn: 'root'
})
export class TranslationsService {
  private locale: string;
  // This is more of a placeholder for future functionality
  private isRTL: boolean;

  getTranslations(packageName: string) {
    return new Observable(observer => {
      let observable = this.apiService.get<ITranslations>('Translations/' + packageName);

      observable.subscribe(result => {
        this.locale = result.resolvedLocale;
        this.isRTL = result.isRTL;

        observer.next(result.translations);
        observer.complete();
      });
    });
  }

  constructor(private apiService: APIRequestService) {
    this.locale = navigator.language;
    this.isRTL = false;
  }
}
