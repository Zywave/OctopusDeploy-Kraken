define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'PROGRESS',
        description: 'Gets the deplyoment progress from Octopus for all projects in a release batch.',
        usage: 'PROGRESS idOrName',
        main: function (idOrName) {
            if (!idOrName) {
                this.shell.writeLine('Project batch id or name required', 'error');
                return;
            }

            return releaseBatchesService.getProgression(idOrName).then(function (data) {
                this.shell.writeLine('Progression for batch ' + idOrName);
                this.shell.writeLine();
                this.shell.writeTable(data, ['projectId:20:project id', 'environmentId:20:environment', 'releaseVersion:20:release version', 'state:*:state'], true);
                return data;
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches']
    });

});