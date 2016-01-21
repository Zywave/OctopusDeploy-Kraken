define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesServices) {

    return new cmdr.Definition({
        name: 'RMBATCH',
        description: 'Deletes a release batch by id.',
        usage: 'RMBATCH batchIdOrName',
        main: function (batchIdOrName) {
            if (!batchIdOrName) {
                this.shell.writeLine('Project batch id or name required', 'error');
                return;
            }
            var deferred = this.defer();
            this.shell.write('Are you sure you want to delete this release batch (y/n)? ', 'warning');
            this.shell.readLine(function (value) {
                if (value.toLowerCase() !== 'y') {
                    deferred.resolve();
                    return;
                }
                releaseBatchesServices.deleteReleaseBatch(batchIdOrName).then(function () {
                    this.shell.writeLine('Release batch deleted', 'success');
                    deferred.resolve();
                }.bind(this)).fail(function (xhr, error, message) {
                    this.fail(xhr, error, message);
                    deferred.resolve();
                }.bind(this));
            }.bind(this));
            return deferred;
        },
        autocompleteKeys: ['releaseBatches']
    });

});