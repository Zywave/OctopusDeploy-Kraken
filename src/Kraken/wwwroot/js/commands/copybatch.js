define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'COPYBATCH',
        description: 'Copies a release batch.',
        usage: 'COPYBATCH idOrName copyName',
        main: function (idOrName, copyName) {
            if (!idOrName || !copyName) {
                this.shell.writeLine('Project batch name or id, and copy name required', 'error');
                return;
            }

            return releaseBatchesService.copyReleaseBatch(idOrName, copyName).then(function (data) {
                this.shell.writeLine('Batch copied', 'success');
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches']
    });

});