define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'LOCKBATCH',
        description: 'Toggles the locked field on a release batch. If a batch is locked, no modifications can be made.',
        usage: 'LOCKBATCH idOrName [comment]',
        main: function (idOrName, comment) {
            if (!idOrName) {
                this.shell.writeLine('Project batch id or name required', 'error');
                return;
            }

            return releaseBatchesService.lockReleaseBatch(idOrName, comment).then(function (data) {
                var lockedOrUnlockedText = data ? 'locked.' : 'unlocked.';
                this.shell.writeLine('Batch has been successfully ' + lockedOrUnlockedText);
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches']
    });
});