define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'DEPLOYBATCH',
        description: 'Deploys all items in a release batch.',
        usage: 'DEPLOYBATCH batchIdOrName environmentIdOrName [--force]\n\nEnvironment id can be full (Environments-123) or short (123)',
        main: function (batchIdOrName, environmentIdOrName, force) {
            if (!batchIdOrName || !environmentIdOrName) {
                this.shell.writeLine('Batch id or name and environment id or name required', 'error');
                return;
            }
            
            return releaseBatchesService.deployReleaseBatch(batchIdOrName, environmentIdOrName, force === '--force').then(function (data) {
                if (data.successfulItems.length) {
                    this.shell.writeLine();
                    this.shell.writeLine('                $$$$$$$');
                    this.shell.writeLine('            $$$$$$$$$$$$$$');
                    this.shell.writeLine('         $$$$$$$$$$$$$$$$$$');
                    this.shell.writeLine('        $$$$$$$$$$$$$$$$$$$$');
                    this.shell.writeLine('       $$$$$$$$$$$$$$$O$$$$$    $$$$$$');
                    this.shell.writeLine('       $$$$$$$$O$$$$$$$$$$$$   $$$  $$$');
                    this.shell.writeLine('       $$$$$$$$$$$$$$$$$$$$  $$$$    $$');
                    this.shell.writeLine('        $$$$$$$$$$$$$$$$$$$$$$$$');
                    this.shell.writeLine('         $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$');
                    this.shell.writeLine(' $$$$$$     $$$$$$$$$$$$$$$$$$        $$');
                    this.shell.writeLine('$$   $$$$$$$$$$$$$$$$$$$$$$$         $$');
                    this.shell.writeLine(' $$$    $$$$$$$$$$$$$$$$$$$$$$$$$$$$');
                    this.shell.writeLine('     $$$$$$$$  $$$ $$$$$$          $$');
                    this.shell.writeLine('   $$$$       $$$  $$$ $$$      $$$$');
                    this.shell.writeLine('  $$$       $$$$   $$$  $$$');
                    this.shell.writeLine('   $$$$$   $$$     $$$   $$$$    $$');
                    this.shell.writeLine('    $$$    $$$$$$  $$$    $$$$$$$$');
                    this.shell.writeLine('            $$$     $$$$$   $$$$');
                    this.shell.writeLine();
                    this.shell.writeLine('         RELEASE THE KRAKEN!');
                    this.shell.writeLine();
                    this.shell.writeLine('Deployments for the following projects were created:');
                    data.successfulItems.forEach(function (item) {
                        this.shell.writeLine(item.name, 'success');
                    }.bind(this));
                } else {
                    this.shell.writeLine('No deployments created');
                }
                if (data.failedItems.length) {
                    this.shell.writeLine();
                    this.shell.writeLine('Deployments for the following projects were not created because an error occured while attempting to create them:');
                    data.failedItems.forEach(function (item) {
                        this.shell.writeLine(item.name + ': ' + item.message, 'error');
                    }.bind(this));
                }
                return data;
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches', 'environments']
    });

});