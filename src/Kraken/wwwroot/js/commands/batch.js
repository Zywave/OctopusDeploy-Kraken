define(['cmdr', 'services/releaseBatches'], function(cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'BATCH',
        description: 'Lists all release batches or lists all projects linked to a batch.',
        usage: 'BATCH/nBATCH [batchid]',
        main: function (batchId) {
            var fail = function(xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this);

            if (batchId) {
                return releaseBatchesService.getReleaseBatch(batchId).then(function (data) {
                    this.shell.writeLine("Projects in batch:");
                    this.shell.writeLine();
                    this.shell.writeTable(data.items, ['projectId:20:project id', 'projectName:50:project name', 'releaseVersion:*:release version'], true);
                }.bind(this)).fail(fail);
            }

            return releaseBatchesService.getReleaseBatches().then(function (data) {
                this.shell.writeLine("Release batches:");
                this.shell.writeLine();
                this.shell.writeTable(data, ['id:10', 'name:*'], true);
            }.bind(this)).fail(fail);
        }
    });

});