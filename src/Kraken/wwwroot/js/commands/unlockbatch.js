define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'UNLOCKBATCH',
        description: 'Unlocks a release batch.',
        usage: 'UNLOCKBATCH idOrName',
        main: function (idOrName) {
            if (!idOrName) {
                this.shell.writeLine('Project batch id or name required', 'error');
                return;
            }

            return releaseBatchesService.unlockReleaseBatch(idOrName).then(function (data) {
                this.shell.writeLine('Batch has been unlocked.', 'success');
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches']
    });
});