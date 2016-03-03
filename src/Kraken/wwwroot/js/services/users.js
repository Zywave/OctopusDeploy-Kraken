define(['jquery', 'context'], function ($, context) {

    return {
        getUser: function (userName) {
            return $.get(context.basePath + 'api/users/' + userName);
        }
    };

});