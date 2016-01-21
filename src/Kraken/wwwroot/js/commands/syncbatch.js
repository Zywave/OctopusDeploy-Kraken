define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'SYNCBATCH',
        description: 'Syncs up a release batch with releases from Octopus.',
        usage: 'SYNCBATCH batchIdOrName [environmentIdOrName]\n\nEnvironment id can be full (Environments-123) or short (123)',
        main: function (batchIdOrName, environmentIdOrName) {
            if (!batchIdOrName) {
                this.shell.writeLine('Batch id or name required', 'error');
                return;
            }
            return releaseBatchesService.syncReleaseBatch(batchIdOrName, environmentIdOrName).then(function (data) {
                this.shell.writeLine('Release batch synced', 'success');
                return data;
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches', 'environments']
    });

});