define(['jquery', 'context'], function ($, context) {

    return {
        getProjects: function (nameFilter) {
            return $.get(context.basePath + 'api/projects', { nameFilter: nameFilter });
        }
    };

});