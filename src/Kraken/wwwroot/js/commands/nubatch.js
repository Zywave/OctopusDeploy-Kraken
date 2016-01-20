define(['cmdr', 'services/releaseBatches'], function(cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'NUBATCH',
        description: 'Makes new releases for every project in a batch from the latest nuget package. Give a version to change from the default version when creating a new release.',
        usage: 'NUBATCH batchid [version]',
        main: function (batchId, version) {
            if (!batchId) {
                this.shell.writeLine('Batch id required', 'error');
                return;
            }

            return releaseBatchesService.createReleases(batchId, version).then(function (data) {
                this.shell.writeLine('Release batch updated and releases created', 'success');
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