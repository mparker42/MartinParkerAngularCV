import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { PortfolioComponent } from './portfolio.component';

let component: PortfolioComponent;
let fixture: ComponentFixture<PortfolioComponent>;

describe('portfolio component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ PortfolioComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(PortfolioComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
