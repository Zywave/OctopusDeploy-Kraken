define(['jquery', 'context'], function ($, context) {

    return {
        getProjects: function () {
            return $.get(context.basePath + 'api/projects');
        }
    };

});