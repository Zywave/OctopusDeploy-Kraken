define(['cmdr', 'bus','services/releaseBatches'], function(cmdr, bus, releaseBatchesServices) {

    return new cmdr.Definition({
        name: 'RMBATCH',
        description: 'Deletes a release batch by id.',
        usage: 'RMBATCH id',
        main: function (id) {
            if (!id) {
                this.shell.writeLine('Project batch id required', 'error');
                return;
            }
            var deferred = this.defer();
            this.shell.write('Are you sure you want to delete this release batch (y/n)? ', 'warning');
            this.shell.readLine(function (value) {
                if (value.toLowerCase() !== 'y') {
                    deferred.resolve();
                    return;
                }
                releaseBatchesServices.deleteReleaseBatch(id).then(function () {
                    bus.publish('releasebatches:delete', id);
                    this.shell.writeLine('Release batch deleted', 'success');
                    deferred.resolve();
                }.bind(this)).fail(function (xhr, error, message) {
                    this.shell.writeLine(message, 'error');
                    this.shell.writeLine('Operation failed', 'error');
                    deferred.resolve();
                }.bind(this));
            }.bind(this));
            return deferred;
        }
    });

});