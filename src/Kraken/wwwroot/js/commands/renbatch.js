define(['cmdr', 'bus', 'services/releaseBatches'], function(cmdr, bus, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'RENBATCH',
        description: 'Renames a release batch.',
        usage: 'RENBATCH id name',
        main: function (id, name) {
            if (!id || !name) {
                this.shell.writeLine('Project batch id and name required', 'error');
                return;
            }
            return releaseBatchesService.putReleaseBatch(id, { id: id, name: name }).then(function () {
                bus.publish('releasebatches:update', id);
                this.shell.writeLine('Batch updated', 'success');
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

});