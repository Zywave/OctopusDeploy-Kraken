define(['jquery', 'context'], function ($, context) {

    return {
        getEnvironments: function(permission) {
            return $.get(context.basePath + 'api/environments/' + (permission || 'None'));
        }
    };

});