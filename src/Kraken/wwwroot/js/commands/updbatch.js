define(['cmdr', 'jquery', 'services/releaseBatches', 'utils/imagePrompt'], function (cmdr, $, releaseBatchesService, imagePrompt) {

    return new cmdr.Definition({
        name: 'UPDBATCH',
        description: 'Updates basic settings for a release batch.',
        usage: 'UPDBATCH batchIdOrName [--name newName] [--description newDescription] [--logo-file|--logo-reset]',
        main: function (batchIdOrName) {
            if (!batchIdOrName) {
                this.shell.writeLine('Project batch id or name required', 'error');
                return;
            }
            
            var args = this.parsed.args.slice(1);
            function getNamedArg(name, flag) {
                var index = args.indexOf(name);
                if (index >= 0) {
                    if (flag) {
                        return true;
                    }
                    if ((index + 1) in args) {
                        return args[index + 1];
                    }
                    throw 'Invalid arguments';
                }
                return null;
            };
            
            var name, description, logoFile, logoReset;
            try
            {
                name = getNamedArg('--name');
                description = getNamedArg('--description');
                logoFile = getNamedArg('--logo-file', true);
                logoReset = getNamedArg('--logo-reset', true);                
            } catch (error) {
                this.shell.writeLine(error, 'error');
                return;
            }
            
            var tasks = [];

            if (name || description) {
                tasks.push(releaseBatchesService.putReleaseBatch(batchIdOrName, {
                    name: name,
                    description: description
                }).then(function () {
                    this.shell.writeLine('Batch name and/or description updated', 'success');
                }.bind(this)).fail(this.fail.bind(this)));
            }

            if (logoReset) {
                tasks.push(releaseBatchesService.putReleaseBatchLogo(batchIdOrName, null).then(function () {
                    this.shell.writeLine('Batch logo reset', 'success');
                }.bind(this)).fail(this.fail.bind(this)));
            } else if (logoFile) {
                tasks.push(imagePrompt().then(function (logo) {
                    return releaseBatchesService.putReleaseBatchLogo(batchIdOrName, logo).then(function () {
                        this.shell.writeLine('Batch logo set', 'success');
                    }.bind(this));
                }.bind(this)).fail(this.fail.bind(this)));
            }

            return $.when.apply($, tasks);
        },
        autocompleteKeys: ['releaseBatches']
    });

});