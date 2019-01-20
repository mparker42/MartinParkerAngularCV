"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var APIRequestService = /** @class */ (function () {
    function APIRequestService(http) {
        this.http = http;
    }
    APIRequestService.prototype.handleError = function (error) {
        if (error.error instanceof ErrorEvent) {
            // A client-side or network error occurred. Handle it accordingly.
            console.error('An error occurred:', error.error.message);
        }
        else {
            // The backend returned an unsuccessful response code.
            // The response body may contain clues as to what went wrong,
            console.error("Backend returned code " + error.status + ", " +
                ("body was: " + error.error));
        }
        // return an observable with a user-facing error message
        return rxjs_1.throwError('You failed to communicate with the server; please try again later.');
    };
    ;
    APIRequestService.prototype.get = function (path) {
        return this.http.get('/api/' + path)
            .pipe(operators_1.retry(5), operators_1.catchError(this.handleError));
    };
    return APIRequestService;
}());
exports.APIRequestService = APIRequestService;
//# sourceMappingURL=api-request-service.js.map