requirejs.config({
    baseUrl: 'js',
    paths: {
        html: '../html',

        text : '../lib/text/text',
        cmdr: '../lib/cmdrjs/dist/cmdr',
        jquery: '../lib/jquery/dist/jquery',
        knockout: '../lib/knockout/dist/knockout',
        bootstrap: '../lib/bootstrap/dist/js/boostrap'
    }
});

define('context', window.appContext);

requirejs(['knockout', 'componentLoader', 'shell'],
function (ko) {
    ko.applyBindings();
});