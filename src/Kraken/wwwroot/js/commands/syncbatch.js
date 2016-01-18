define(['cmdr', 'bus', 'services/releaseBatches'], function(cmdr, bus, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'SYNCBATCH',
        description: 'Syncs up a release batch with releases from Octopus.',
        usage: 'SYNCBATCH batchId [environmentIdOrName]',
        main: function (batchId, environmentIdOrName) {
            if (!batchId) {
                this.shell.writeLine('Batch id required', 'error');
                return;
            }
            return releaseBatchesService.syncReleaseBatch(batchId, environmentIdOrName).then(function (data) {
                bus.publish('releasebatches:update', batchId);
                this.shell.writeLine('Release batch updated', 'success');
                this.shell.writeLine();
                this.shell.writeLine("Batch name: " + data.name);
                this.shell.writeLine("Projects in batch:");
                this.shell.writeLine();
                this.shell.writeTable(data.items, ['projectId:20:project id', 'projectName:50:project name', 'releaseVersion:*:release version'], true);
                return data;
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                if (xhr.responseText) {
                    this.shell.writeLine(xhr.responseText, 'error');
                }
                this.shell.writeLine('Operation Failed', 'error');
            }.bind(this));
        }
    });

});