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
        putReleaseBatchLogo: function (idOrName, releaseBatchLogo) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/logo',
                data: JSON.stringify(releaseBatchLogo),
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
        linkProject: function (idOrName, projectIdOrSlugOrName, releaseVersion) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/linkproject',
                data: JSON.stringify({ projectIdOrSlugOrName: projectIdOrSlugOrName, releaseVersion: releaseVersion }),
                contentType: 'application/json'
            }).then(function () {
                bus.publish('releasebatches:update', idOrName);
            });
        },
        unlinkProject: function (idOrName, projectIdOrSlugOrName) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/unlinkproject',
                data: JSON.stringify(projectIdOrSlugOrName),
                contentType: 'application/json'
            }).then(function () {
                bus.publish('releasebatches:update', idOrName);
            });
        },
        syncReleaseBatch: function (idOrName, environmentIdOrName) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/sync',
                data: JSON.stringify(environmentIdOrName),
                contentType: 'application/json'
            }).then(function () {
                bus.publish('releasebatches:sync', idOrName);
            });;
        },
        deployReleaseBatch: function (idOrName, environmentIdOrName, forceRedeploy) {
            return $.ajax({
                type: 'POST',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/deploy?forceRedeploy=' + !!forceRedeploy,
                data: JSON.stringify(environmentIdOrName),
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
        previewReleases: function (idOrName) {
            return $.ajax({
                type: 'GET',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/previewreleases',
                contentType: 'application/json'
            }).then(function (data) {
                return data;
            });
        },
        getProgression: function (idOrName) {
            return $.ajax({
                type: 'GET',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/getprogression',
                traditional: true,
                contentType: 'application/json'
            }).then(function (data) {
                return data;
            });
        },
        getBatchEnvironments: function (idOrName) {
            return $.get(context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/getbatchenvironments');
        },
        lockReleaseBatch: function(idOrName, comment) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + encodeURI(idOrName) + '/lockreleasebatch',
                data: JSON.stringify(comment),
                contentType: 'application/json'
            }).then(function(data) {
                return data;
            });
        }
    };

});