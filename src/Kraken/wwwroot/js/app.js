requirejs.config({
    paths: {
        html: '../html',

        jquery: ['//ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min', '../lib/jquery/dist/jquery.min'],
        knockout: ['//ajax.aspnetcdn.com/ajax/knockout/knockout-3.4.0', '../lib/knockout/dist/knockout'],
        bootstrap: ['//ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/bootstrap.min', '../lib/bootstrap/dist/js/bootstrap.min'],
        cmdr: '../lib/cmdrjs/dist/cmdr.min',
        moment: '../lib/moment/min/moment.min',
        select2: '../lib/select2/dist/js/select2',
        koselect2: '../lib/knockout-select2v4/dist/knockout-select2',
        text: '../lib/text/text',
        underscore: '../lib/underscore/underscore'
    },
    shim: {
        underscore: {
            exports: '_'
        }
    }
});

define('context', window.appContext);

requirejs(['knockout', 'componentLoader', 'shell'],
function (ko) {
    ko.applyBindings();
});