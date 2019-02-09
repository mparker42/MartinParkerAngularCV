import { BrowserModule } from '@angular/platform-browser';
import { LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import localeEnGB from '@angular/common/locales/en-GB';


import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NavigationMenuComponent } from './navigation-menu/navigation-menu.component';
import { AboutComponent } from "./about/about.component";
import { LayoutModule } from '@angular/cdk/layout';
import { MatToolbarModule, MatButtonModule, MatSidenavModule, MatIconModule, MatListModule } from '@angular/material';
import { APIRequestService } from './services/api-request-service';
import { TranslationsService } from './services/translation-service';
import { registerLocaleData } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';

registerLocaleData(localeEnGB, 'en-GB')

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavigationMenuComponent,
    AboutComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' }
    ]),
    LayoutModule,
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatIconModule,
    MatListModule,
    MatExpansionModule
  ],
  providers: [
    APIRequestService,
    TranslationsService,
    { provide: LOCALE_ID, useValue: 'en-GB' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
