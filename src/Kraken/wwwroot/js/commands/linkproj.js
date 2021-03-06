﻿define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService, helpers) {

    return new cmdr.Definition({
        name: 'LINKPROJ',
        description: 'Links an Octopus project to a release batch and optionally sets the associated release number.',
        usage: 'LINKPROJ batchIdOrName projectIdOrSlugOrName [releaseVersion]\n\nProject id can be full (Projects-123) or short (123)',
        main: function (batchIdOrName, projectIdOrSlugOrName, releaseVersion) {
            if (!batchIdOrName || !projectIdOrSlugOrName) {
                this.shell.writeLine('Batch id or name and project id or slug or name required', 'error');
                return;
            }
            return releaseBatchesService.linkProject(batchIdOrName, projectIdOrSlugOrName, releaseVersion).then(function () {
                this.shell.writeLine('Release batch updated', 'success');
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches', 'projects']
    });

});