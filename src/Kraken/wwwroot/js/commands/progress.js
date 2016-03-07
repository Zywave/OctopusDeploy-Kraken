define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'PROGRESS',
        description: 'Gets the deplyoment progress from Octopus for all projects in a release batch.',
        usage: 'PROGRESS name',
        main: function (name) {
            if (!name) {
                this.shell.writeLine('Project batch name required', 'error');
                return;
            }

            return releaseBatchesService.getProgression(name).then(function (data) {
                this.shell.writeLine('Progression for: ' + name);
                this.shell.writeLine();
                //this.shell.writeTable(data.releases, ['projectId:20:project id', 'projectName:50:project name', 'releaseVersion:*:release version'], true);
                //this.shell.writeLine();

                return data;
            }.bind(this)).fail(this.fail.bind(this));
        }
    });

});