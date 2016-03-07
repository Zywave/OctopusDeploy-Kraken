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
                    _.each(_.keys(data), function (projectId) {
                        if (data[projectId].releases) {
                            if (!environmentIds.length) {
                                environmentIds = _.pluck(data[projectId].environments, 'id');
                            }
                            _.each(environmentIds, function(environmentId) {
                                var release = _.find(data[projectId].releases, function(releases) {
                                    return releases.deployments[environmentId.toLowerCase()];
                                });
                                if (release) {
                                    var state = release.deployments[environmentId.toLowerCase()].state;
                                    if (state === 6) { // 'Success'
                                        state = 'Success';
                                    } else if (state === 1 || state === 4 || state === 7) { // 'Canceled' || 'Failed' || 'TimedOut'
                                        state = 'Failed';
                                    } else { // 'Cancelling' || 'Executing' || 'Queued'
                                        state = 'Executing';
                                    }

                                    var rowData = {
                                        projectId: projectId,
                                        environmentId: environmentId,
                                        state: state
                                    };
                                    tableData.push(rowData);
                                }
                            });
                        }
                    }.bind(this));
                    this.shell.writeTable(tableData, ['projectId:20:project id', 'environmentId:20:environment', 'state:*:state'], true);
                }
                return data;
            }.bind(this)).fail(this.fail.bind(this));
        }
    });

});