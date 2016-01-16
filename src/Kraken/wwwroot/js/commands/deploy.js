define(['cmdr', 'bus', 'services/releaseBatches'], function (cmdr, bus, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'DEPLOY',
        description: 'Deploys all items in a release batch',
        usage: 'DEPLOY batchId environmentId',
        main: function (batchId, environmentId) {
            if (!batchId || !environmentId) {
                this.shell.writeLine('Batch id and environmentId required', 'error');
                return;
            }
            return releaseBatchesService.deployReleaseBatch(batchId, environmentId).then(function (data) {
                bus.publish('releasebatches:deploy', batchId);
                this.shell.writeLine('Release batch deployed', 'success');
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

});