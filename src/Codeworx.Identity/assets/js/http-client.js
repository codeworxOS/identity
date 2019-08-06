(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var HttpClient = (function () {
        function HttpClient() {
        }
        HttpClient.prototype.getData = function (url) {
            return null;
        };
        return HttpClient;
    }());
    exports.HttpClient = HttpClient;
});
