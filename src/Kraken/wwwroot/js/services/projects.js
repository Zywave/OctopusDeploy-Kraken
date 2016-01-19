define(['jquery', 'context'], function ($, context) {

    return {
        getProjects: function (searchQuery) {
            return $.get(context.basePath + 'api/projects', { searchQuery: searchQuery });
        }
    };

});