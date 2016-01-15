define(['jquery', 'bus', 'context'], function ($, bus, context) {

    return {
        getReleaseBatches: function() {
            return $.get(context.basePath + 'api/releasebatches');
        },
        getReleaseBatch: function (id) {
            return $.get(context.basePath + 'api/releasebatches/' + id);
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
        putReleaseBatch: function (id, releaseBatch) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + id,
                data: JSON.stringify(releaseBatch),
                contentType: 'application/json'
            }).then(function (data) {
                bus.publish('releasebatches:add', data.id);
                return data;
            });
        },
        deleteReleaseBatch: function(id) {
            return $.ajax({
                url: context.basePath + 'api/releasebatches/' + id,
                type: 'DELETE'
            }).then(function (data) {
                bus.publish('releasebatches:delete', id);
                return data;
            });
        },
        linkProject: function(batchId, projectId) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + batchId + '/linkproject',
                data: JSON.stringify(projectId),
                contentType: 'application/json'
            });
        },
        unlinkProject: function(batchId, projectId) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + batchId + '/unlinkproject',
                data: JSON.stringify(projectId),
                contentType: 'application/json'
            });
        },
        syncReleaseBatch: function (batchId, environmentId) {
            return $.ajax({
                type: 'PUT',
                url: context.basePath + 'api/releasebatches/' + batchId + '/sync',
                data: JSON.stringify(environmentId),
                contentType: 'application/json'
            });
        }
    };

});