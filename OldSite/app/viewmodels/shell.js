define(['plugins/router', 'durandal/app', 'i18next', 'jstr', 'knockout'], function (router, app, i18, jstr, ko) {
    var msg,
        staticMsg,
        displayStartupMsg,
        startupMsgClosed = ko.observable(false),
        neverShowStartupMsgRaw = jstr.get('neverShowStartMsg'),
        neverShowStartupMsg = ko.observable(neverShowStartupMsgRaw);

    displayStartupMsg = ko.computed(function () {
        if (startupMsgClosed() || neverShowStartupMsgRaw) {
            return false;
        }
        return true;
    });

    staticMsg = ko.computed(function () {
        return displayStartupMsg();
    });

    msg = ko.computed(function () {
        var msgHtml = '';

        if (displayStartupMsg()) {
            msgHtml += i18.t('itmOpeningMsg');
        }
        return msgHtml;
    });

    return {
        router: router,
        msg: msg,
        msgClass: ko.computed(function () {
            var msgCSS = 'messageHost ';

            if (msg() == '') {
                msgCSS += 'empty ';
                $('.msgIndent').css('margin-top', 5);
            }
            if (staticMsg()) {
                msgCSS += 'static ';
            }
            if (displayStartupMsg()) {
                msgCSS += 'inform ';
            }

            return msgCSS;
        }),
        closeMsg: function(){
            if (!startupMsgClosed()) {
                startupMsgClosed(true);
            }
        },
        activate: function () {
            $(window).resize(function () {
                if (msg() == '') {
                    $('.msgIndent').css('margin-top', 5);
                }
                else {
                    var msgHeight = $('.messageHost').outerHeight();
                    //msgHeight -= 10;
                    $('.msgIndent').css('margin-top', msgHeight);
                    $('.messageHost').css('margin-top', (-msgHeight) - 10);
                }
            });

            router.map([
                { route: '', title:i18.t('home'), moduleId: 'viewmodels/search', nav: true },
                { route: 'flickr', moduleId: 'viewmodels/flickr', nav: true }
            ]).buildNavigationModel();
            
            return router.activate();
        },
        compositionComplete: function () {
            if (msg() == '') {
                $('.msgIndent').css('margin-top', 5);
            }
            else {
                var msgHeight = $('.messageHost').outerHeight();
                //msgHeight -= 10;
                $('.msgIndent').css('margin-top', msgHeight);
                $('.messageHost').css('margin-top', (-msgHeight) - 10);
            }
        },
        hideStartCheckboxCSS: ko.computed(function () {
            var hideStart = 'checkbox fa ';

            if (neverShowStartupMsg()) {
                hideStart += 'fa-check-square-o ';
            } else {
                hideStart += 'fa-square-o ';
            }

            return hideStart;
        }),
        hideStartCSS: ko.computed(function () {
            var hideStart = 'hideStart ';

            if(!displayStartupMsg()){
                hideStart += 'hidden ';
            }

            return hideStart;
        }),
        hideStartHtml: ko.computed(function () {
            return i18.t('itmNeverShow');
        }),
        hideStartClick: function () {
            var hideStart = !neverShowStartupMsg();
            neverShowStartupMsg(hideStart);
            jstr.set('neverShowStartMsg', hideStart);
            neverShowStartupMsgRaw = hideStart;
        },
        helpClick: function () {

        }
    };
});