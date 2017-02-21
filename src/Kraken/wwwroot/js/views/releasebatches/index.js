define(['knockout', 'shell', 'bus', 'services/releaseBatches', 'context'], function(ko, shell, bus, releaseBatchesService, context) {
    return function () {

        this.manage = function () {
            shell.open();
        }.bind(this);

        function AddBatchModal() {
            this.show = ko.observable(false);
            this.invalid = ko.observable(false);
            this.batchName = ko.observable('');

            var deferred;

            this.submit = function () {
                if (!this.batchName()) {
                    this.invalid(true);
                    return;
                }
                if (deferred) {
                    deferred.resolve(this.batchName());
                    deferred = null;
                }
                this.close();
            }.bind(this);

            this.open = function () {
                this.show(true);
                if (!deferred) {
                    deferred = $.Deferred();
                }
                return deferred.promise();
            }.bind(this);

            this.close = function () {
                this.invalid(false);
                this.batchName('');
                this.show(false);
                if (deferred) {
                    deferred.resolve(false);
                    deferred = null;
                }
            }.bind(this);
        };

        this.addBatchModal = new AddBatchModal();

        this.addBatch = function () {
            this.addBatchModal.open().then(function (batchName) {
                if (batchName) {
                    shell.execute('MKBATCH', batchName);
                }
            });
        }.bind(this);

        this.releaseBatches = ko.observableArray();

        var mapReleaseBatch = function(batch) {
            batch.detailsUrl = context.basePath + 'app/releasebatches/details?id=' + batch.id;
            batch.logoUrl = context.basePath + 'api/releasebatches/' + batch.id + '/logo?cb=' + new Date(batch.updateDateTime).getTime();
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