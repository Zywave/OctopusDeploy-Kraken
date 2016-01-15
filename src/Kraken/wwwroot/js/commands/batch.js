define(['cmdr', 'services/releaseBatches'], function(cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'BATCH',
        description: 'Lists all release batches.',
        main: function () {
            return releaseBatchesService.getReleaseBatches().then(function (data) {
                this.shell.writeLine("Release batches:");
                this.shell.writeLine();
                this.shell.writeTable(data, ['id:10', 'name:*'], true);
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

});