(function (b, k) { "function" === typeof define && define.amd ? define(["knockout"], k) : k(b.ko) })(this, function (b) {
    function k(a, f) { var c = m(f.$switchValueAccessor()); return "boolean" == typeof c ? a ? c : !c : "boolean" == typeof a ? a : a instanceof Array ? -1 !== b.utils.arrayIndexOf(a, c) : a == c } function v(a, b) { return !k(a, b) } function w(a) { return function () { return a } } function h(a, f, c) {
        var n = f ? v : k; a || (a = "if"); c || (c = w); return {
            flags: d[a].flags, init: function (f, g, l, x, e) {
                if (!e.$switchSkipNextArray) throw Error("case binding must only be used with a switch binding");
                if (e.$switchIndex !== p) throw Error("case binding cannot be nested"); e.$switchIndex = e.$switchSkipNextArray.push(b.observable(!1)) - 1; e.$caseValue = b.observable(); b.computed(function () { var a = e.$switchIndex, b = a === e.$switchSkipNextArray.length - 1, c, f, d; a && e.$switchSkipNextArray[a - 1]() ? (c = !1, f = !0) : (c = m(g()), c === e.$else ? (c = e.$switchDefault() || b, f = !1) : d = f = c = n(c, e)); e.$caseValue(c); e.$switchSkipNextArray[a](f); d ? e.$switchDefault(!1) : !f && b && e.$switchDefault(!0) }, null, { disposeWhenNodeIsRemoved: f }); if (d[a].init) return d[a].init(f,
                c(e.$caseValue), l, x, e)
            }, update: function (b, f, g, n, e) { if (d[a].update) return d[a].update(b, c(e.$caseValue), g, n, e) }
        }
    } function r(a, b, c) { g.allowedBindings[a] && (g.allowedBindings[c] = !0); return h(a, "casenot" === b) } function y(a, b, c) { return r(b, a, c) } function s(a, b) { d[a] = h("if", b); t[a] = !1; g.allowedBindings[a] = !0; d[a].makeSubkeyHandler = y; d[a].getNamespacedHandler = r } var p; if (!b.virtualElements) throw Error("Switch-case requires at least Knockout 2.1"); var g = b.virtualElements, q = b.bindingFlags || {}, t = b.bindingRewriteValidators ||
    b.jsonExpressionRewriting.bindingRewriteValidators, m = b.utils.unwrapObservable, d = b.bindingHandlers, u = {}; d["switch"] = {
        flags: q.contentBind | q.canUseVirtual | q.noValue, init: function (a, f, c, d, k) {
            var h = { $switchSkipNextArray: [], $switchValueAccessor: f, $switchDefault: b.observable(!0), $default: u, $else: u }, l = []; b.computed(function () { var a = m(f()); h.$value = a; b.utils.arrayForEach(l, function (b) { b.$value = a }) }, null, { disposeWhenNodeIsRemoved: a }); for (c = g.firstChild(a) ; a = c;) switch (c = g.nextSibling(a), a.nodeType) {
                case 1: case 8: d =
                k.extend(h), d.$switchIndex = p, b.applyBindings(d, a), d.$switchIndex !== p && l.push(d)
            } return { controlsDescendantBindings: !0 }
        }, preprocess: function (a) { return a || "true" }
    }; t["switch"] = !1; g.allowedBindings["switch"] = !0; s("case"); s("casenot", !0); d["case.visible"] = h("visible"); d["casenot.visible"] = h("visible", !0); d["switch"].makeCaseHandler = h
});