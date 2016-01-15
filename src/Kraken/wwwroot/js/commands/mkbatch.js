define(['cmdr', 'bus', 'services/releaseBatches'], function(cmdr, bus, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'MKBATCH',
        description: 'Creates a new release batch.',
        usage: 'MKBATCH name',
        main: function (name) {
            if (!name) {
                this.shell.writeLine('Project batch name required', 'error');
                return;
            }
            return releaseBatchesService.postReleaseBatch({ name: name }).then(function (data) {
                bus.publish('releasebatches:add', data.id);
                this.shell.writeLine('Batch created', 'success');
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

});