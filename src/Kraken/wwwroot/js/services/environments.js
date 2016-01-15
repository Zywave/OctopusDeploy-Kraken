define(['jquery', 'context'], function ($, context) {

    return {
        getEnvironments: function() {
            return $.get(context.basePath + 'api/environments');
        }
    };

});