import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { FilterComponent } from './filter.component';

let component: FilterComponent;
let fixture: ComponentFixture<FilterComponent>;

describe('filter component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ FilterComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(FilterComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
