define(['jquery', 'bus', 'context'], function ($, bus, context) {

    return {
        getReleaseBatches: function() {
            return $.get(context.basePath + 'api/releasebatches');
        },
        getReleaseBatch: function (idOrName) {
            return $.get(context.basePath + 'api/releasebatches/' + encodeURI(idOrName));
        },
        postReleaseBatch: function (releaseBatch) {
            return $.ajax({
                type: 'POST',
                url: context.basePath + 'api/releasebatches',
                data: JSON.stringify(releaseBatch),
                contentType: 'application/json'
            }).then(function(data) {
                bus.publish('releasebatches:add', data.id);
                return data;
            });
        },
        putReleaseBatch: function (idOrName, releaseBatch) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName),
                data: JSON.stringify(releaseBatch),
                contentType: 'application/json'
            }).then(function (data) {
                bus.publish('releasebatches:update', idOrName);
                return data;
            });
        },
        deleteReleaseBatch: function (idOrName) {
            return $.ajax({
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName),
                type: 'DELETE'
            }).then(function (data) {
                bus.publish('releasebatches:delete', idOrName);
                return data;
            });
        },
        linkProject: function (idOrName, projectId) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/linkproject',
                data: JSON.stringify(projectId),
                contentType: 'application/json'
            }).then(function () {
                bus.publish('releasebatches:update', idOrName);
            });
        },
        unlinkProject: function (idOrName, projectId) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/unlinkproject',
                data: JSON.stringify(projectId),
                contentType: 'application/json'
            }).then(function () {
                bus.publish('releasebatches:update', idOrName);
            });
        },
        syncReleaseBatch: function (idOrName, environmentId) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/sync',
                data: JSON.stringify(environmentId),
                contentType: 'application/json'
            }).then(function () {
                bus.publish('releasebatches:sync', idOrName);
            });;
        },
        deployReleaseBatch: function (idOrName, environmentId, allowRedeploy) {
            return $.ajax({
                type: 'POST',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/deploy',
                data: {
                    environmentId: environmentId,
                    allowRedeploy: !!allowRedeploy
                },
                contentType: 'application/json'
            }).then(function() {
                bus.publish('releasebatches:deploy', idOrName);
            });
        },
        createReleases: function (idOrName, releases) {
            return $.ajax({
                type: 'POST',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/createreleases',
                data: JSON.stringify(releases),
                contentType: 'application/json'
            }).then(function () {
                bus.publish('releasebatches:sync', idOrName);
            });
        },
        getNextReleases: function (idOrName) {
            return $.ajax({
                type: 'GET',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/getnextreleases',
                contentType: 'application/json'
            }).then(function (data) {
                return data;
            });
        }
    };

});