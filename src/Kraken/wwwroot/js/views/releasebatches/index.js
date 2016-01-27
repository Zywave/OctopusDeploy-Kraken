define(['knockout', 'shell', 'bus', 'services/releaseBatches', 'context'], function(ko, shell, bus, releaseBatchesService, context) {
    return function(params) {

        this.releaseBatches = ko.observableArray();

        this.manage = function () {
            shell.open();
        }.bind(this);

        var mapReleaseBatch = function(batch) {
            batch.detailsUrl = context.basePath + 'app/releasebatches/details?id=' + batch.id;
            batch.logoUrl = context.basePath + 'images/batch-logo.png';
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