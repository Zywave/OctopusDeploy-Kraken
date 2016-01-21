define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'NUBATCH',
        description: 'Makes a new release for every project in a batch from the latest nuget packages.',
        usage: 'NUBATCH batchIdOrName [version]',
        main: function (batchIdOrName, version) {
            if (!batchIdOrName) {
                this.shell.writeLine('Batch id or name required', 'error');
                return;
            }

            return releaseBatchesService.createReleases(batchIdOrName, version).then(function (data) {
                this.shell.writeLine('Releases created and batch synced with latest', 'success');
                return data;
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches']
    });

});