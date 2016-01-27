define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'MKBATCH',
        description: 'Creates a new release batch.',
        usage: 'MKBATCH name [description]',
        main: function (name, description) {
            if (!name) {
                this.shell.writeLine('Project batch name required', 'error');
                return;
            }

            return releaseBatchesService.postReleaseBatch({ name: name, description: description }).then(function (data) {
                this.shell.writeLine('Batch created', 'success');
            }.bind(this)).fail(this.fail.bind(this));
        }
    });

});