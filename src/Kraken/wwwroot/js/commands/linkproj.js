define(['cmdr', 'bus', 'services/releaseBatches'], function(cmdr, bus, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'LINKPROJ',
        description: 'Links an Octopus project to a release batch.',
        usage: 'LINKPROJ batchId projectId',
        main: function (batchId, projectId) {
            if (!batchId || !projectId) {
                this.shell.writeLine('Batch id and project id required', 'error');
                return;
            }
            return releaseBatchesService.linkProject(batchId, projectId).then(function () {
                bus.publish('releasebatches:update', batchId);
                this.shell.writeLine('Release batch updated', 'success');
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