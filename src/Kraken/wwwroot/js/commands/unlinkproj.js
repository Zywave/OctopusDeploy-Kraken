define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'UNLINKPROJ',
        description: 'Unlinks an Octopus project from a release batch.',
        usage: 'UNLINKPROJ batchIdOrName projectIdOrSlugOrName\n\nProject id can be full (Projects-123) or short (123)',
        main: function (batchIdOrName, projectIdOrSlugOrName) {
            if (!batchIdOrName || !projectIdOrSlugOrName) {
                this.shell.writeLine('Batch id or name and project id or slug or name required', 'error');
                return;
            }
            return releaseBatchesService.unlinkProject(batchIdOrName, projectIdOrSlugOrName).then(function () {
                this.shell.writeLine('Release batch updated', 'success');
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches', 'projects']
    });

});