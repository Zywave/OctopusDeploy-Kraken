define(['knockout', 'bus', 'services/releaseBatches', 'context'], function(ko, bus, releaseBatchesService, context) {
    return function(params) {

        this.releaseBatches = ko.observableArray();

        var mapReleaseBatch = function(batch) {
            batch.viewUrl = context.basePath + 'app/releasebatches/details?id=' + batch.id;
            return batch;
        }

        var loadReleaseBatches = function() {
            releaseBatchesService.getReleaseBatches().then(function(data) {
                this.releaseBatches($.map(data, mapReleaseBatch));
            }.bind(this));
        }.bind(this);

        bus.subscribe('releasebatches:add', loadReleaseBatches);
        bus.subscribe('releasebatches:update', loadReleaseBatches);
        bus.subscribe('releasebatches:delete', loadReleaseBatches);

        loadReleaseBatches();
    };
});