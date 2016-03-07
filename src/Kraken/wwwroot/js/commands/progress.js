define(['cmdr', 'services/releaseBatches', 'underscore'], function (cmdr, releaseBatchesService, _) {

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
                // TODO display progress in shell
                //this.shell.writeLine('Progression for batch idOrName ' + name);
                //this.shell.writeLine();
                //_.each(_.keys(data), function (projectId) {
                //    if (data[projectId].releases) {
                //        this.shell.writeLine(projectId);
                //        //this.shell.writeTable(data.releases, ['projectId:20:project id', 'projectName:50:project name', 'releaseVersion:*:release version'], true);
                //    }
                //}.bind(this));
                //this.shell.writeTable(data.releases, ['projectId:20:project id', 'projectName:50:project name', 'releaseVersion:*:release version'], true);
                //this.shell.writeLine();

                return data;
            }.bind(this)).fail(this.fail.bind(this));
        }
    });

});