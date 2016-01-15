define(['cmdr', 'bus', 'services/releaseBatches'], function (cmdr, bus, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'UNLINKPROJ',
        description: 'Unlinks an Octopus project from a release batch.',
        usage: 'UNLINKPROJ batchId projectId',
        main: function (batchId, projectId) {
            if (!batchId || !projectId) {
                this.shell.writeLine('Batch id and project id required', 'error');
                return;
            }
            return releaseBatchesService.unlinkProject(batchId, projectId).then(function () {
                bus.publish('releasebatches:update', batchId);
                this.shell.writeLine('Release batch updated', 'success');
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

});