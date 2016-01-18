define(['cmdr', 'bus', 'services/releaseBatches'], function(cmdr, bus, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'SYNCBATCH',
        description: 'Syncs up a release batch with releases from Octopus.',
        usage: 'SYNCBATCH batchId [environmentIdOrName]\n\nEnvironment id can be full (Environments-123) or short (123)',
        main: function (batchId, environmentIdOrName) {
            if (!batchId) {
                this.shell.writeLine('Batch id required', 'error');
                return;
            }
            return releaseBatchesService.syncReleaseBatch(batchId, environmentIdOrName).then(function (data) {
                bus.publish('releasebatches:update', batchId);
                this.shell.writeLine('Release batch synced', 'success');
                return data;
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                if (xhr.responseText) {
                    this.shell.writeLine(xhr.responseText, 'error');
                }
                this.shell.writeLine('Operation Failed', 'error');
            }.bind(this));
        }
    });

});