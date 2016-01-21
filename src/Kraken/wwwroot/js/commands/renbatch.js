define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'RENBATCH',
        description: 'Renames a release batch.',
        usage: 'RENBATCH batchIdOrName newName',
        main: function (batchIdOrName, name) {
            if (!batchIdOrName || !name) {
                this.shell.writeLine('Project batch id or name and new name required', 'error');
                return;
            }
            return releaseBatchesService.putReleaseBatch(batchIdOrName, { name: name }).then(function () {
                this.shell.writeLine('Batch updated', 'success');
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches']
    });

});