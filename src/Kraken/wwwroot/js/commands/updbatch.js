define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'UPDBATCH',
        description: 'Updates basic settings for a release batch.',
        usage: 'UPDBATCH batchIdOrName [-name newName] [-description newDescription]',
        main: function (batchIdOrName) {
            if (!batchIdOrName) {
                this.shell.writeLine('Project batch id or name required', 'error');
                return;
            }
            
            var args = this.parsed.args.slice(1);
            function getNamedArg(name) {
                var index = args.indexOf(name);
                if (index >= 0) {
                    if ((index + 1) in args) {
                        return args[index + 1];
                    }
                    else {
                        throw 'Invalid arguments';
                    }
                }
                return '__IGNORE__';
            };

            var data;
            try
            {
                data = {
                    name: getNamedArg('-name'),
                    description: getNamedArg('-description')
                };
            } catch (error) {
                this.shell.writeLine(error, 'error');
                return;
            }

            return releaseBatchesService.putReleaseBatch(batchIdOrName, data).then(function () {
                this.shell.writeLine('Batch updated', 'success');
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches']
    });

});