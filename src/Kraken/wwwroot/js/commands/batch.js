define(['cmdr', 'moment', 'services/releaseBatches'], function(cmdr, moment, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'BATCH',
        description: 'Lists all release batches or lists all projects linked to a batch.',
        usage: 'BATCH/nBATCH [batchid]',
        main: function (batchId) {
            var fail = function(xhr, error, message) {
                this.shell.writeLine(message, 'error');
                if (xhr.responseText) {
                    this.shell.writeLine(xhr.responseText, 'error');
                }
                this.shell.writeLine('Operation Failed', 'error');
            }.bind(this);

            if (batchId) {
                return releaseBatchesService.getReleaseBatch(batchId).then(function (data) {
                    this.shell.writeLine(data.name);
                    this.shell.writeLine();
                    this.shell.writeTable(data.items, ['projectId:20:project id', 'projectName:50:project name', 'releaseVersion:*:release version'], true);
                    this.shell.writeLine();
                    if (data.updateDateTime) {
                        this.shell.writeLine("Last updated by " + data.updateUserName + " at " + moment(data.updateDateTime).format('l LTS'));
                    }
                    if (data.syncDateTime) {
                        this.shell.writeLine("Last synced to " + data.syncEnvironmentName + " by " + data.syncUserName + " at " + moment(data.syncDateTime).format('l LTS'));
                    }
                    if (data.deployDateTime) {
                        this.shell.writeLine("Last deployed to " + data.deployEnvironmentName + " by " + data.deployUserName + " at " + moment(data.deployDateTime).format('l LTS'));
                    }
                }.bind(this)).fail(fail);
            }

            return releaseBatchesService.getReleaseBatches().then(function (data) {
                this.shell.writeLine("Release batches:");
                this.shell.writeLine();
                var displayData = data.map(function (item) {
                    item.displaySyncDateTime = item.syncDateTime ? moment(item.syncDateTime).format('l LTS') : '';
                    item.displayDeployDateTime = item.deployDateTime ? moment(item.deployDateTime).format('l LTS') : '';
                    return item;
                });
                this.shell.writeTable(displayData, ['id:10', 'name:30', 'syncEnvironmentName:15:sync env', 'displaySyncDateTime:25:sync timestamp', 'deployEnvironmentName:15:deploy env', 'displayDeployDateTime:*:deploy timestamp'], true);
            }.bind(this)).fail(fail);
        }
    });

});