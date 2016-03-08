define(['cmdr', 'services/releaseBatches', 'underscore'], function (cmdr, releaseBatchesService, _) {

    return new cmdr.Definition({
        name: 'PROGRESS',
        description: 'Gets the deplyoment progress from Octopus for all projects in a release batch. If --suppressmessage is passed, won\'t display anything in shell',
        usage: 'PROGRESS name [--suppressmessage]',
        main: function (name, suppress) {
            if (!name) {
                this.shell.writeLine('Project batch name required', 'error');
                return;
            }

            return releaseBatchesService.getProgression(name).then(function (data) {
                if (suppress !== '--suppressmessage') {
                    var tableData = [];
                    var environmentIds = [];
                    this.shell.writeLine('Progression for batch idOrName ' + name);
                    this.shell.writeLine();
                    this.shell.writeTable(data, ['projectId:20:project id', 'environmentId:20:environment', 'releaseId:20:release id', 'state:*:state'], true);
                }
                return data;
            }.bind(this)).fail(this.fail.bind(this));
        }
    });

});