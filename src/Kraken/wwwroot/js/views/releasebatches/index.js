define(['knockout', 'shell', 'bus', 'services/releaseBatches', 'context'], function(ko, shell, bus, releaseBatchesService, context) {
    return function(params) {

        this.releaseBatches = ko.observableArray();
        this.showAddBatchModal = ko.observable(false);
        var showAddBatchModal = this.showAddBatchModal;
        this.addBatchModal = {
            invalid: ko.observable(false),
            batchName: ko.observable(''),
            addBatch: function () {
                if (this.addBatchModal.invalid() || !this.addBatchModal.batchName()) {
                    this.addBatchModal.invalid(true);
                    return;
                }
                shell.open();
                shell.execute('MKBATCH', this.addBatchModal.batchName());
                this.addBatchModal.closeModal();
            },
            closeModal: function () {
                this.addBatchModal.invalid(false);
                this.addBatchModal.batchName('');
                showAddBatchModal(false);
            }
        };

        this.manage = function () {
            shell.open();
        }.bind(this);

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