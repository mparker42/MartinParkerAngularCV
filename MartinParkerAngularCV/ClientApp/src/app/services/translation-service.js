"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var TranslationsService = /** @class */ (function () {
    function TranslationsService(apiService) {
        this.apiService = apiService;
    }
    TranslationsService.prototype.getTranslations = function (packageName) {
        return this.apiService.get('Translations/' + packageName);
    };
    return TranslationsService;
}());
exports.TranslationsService = TranslationsService;
//# sourceMappingURL=translation-service.js.map