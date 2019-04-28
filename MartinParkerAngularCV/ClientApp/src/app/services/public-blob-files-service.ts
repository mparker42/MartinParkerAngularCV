import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { APIRequestService } from './api-request-service';
import { IConfiguration } from '../interfaces/configuration';

@Injectable({
  providedIn: 'root',
})
export class PublicBlobFilesService {
  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an observable with a user-facing error message
    return throwError(
      'You failed to communicate with the server; please try again later.');
  };

  blobContainerURL: string = null

  async get<T>(path: string) {
    if (this.blobContainerURL === null)
      this.blobContainerURL = (await this.apiService.get<IConfiguration>('Configuration/publicBlobStoreURL').toPromise()).value;

    return await this.http.get<T>(this.blobContainerURL + path)
      .pipe(
        retry(5),
        catchError(this.handleError)
      ).toPromise();
  }

  constructor(private http: HttpClient, private apiService: APIRequestService) {
  }
}
